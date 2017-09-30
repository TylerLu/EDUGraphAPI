/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using EDUGraphAPI.Data;
using EDUGraphAPI.Utils;
using EDUGraphAPI.Web.Models;
using EDUGraphAPI.Web.ViewModels;
using Microsoft.Education;
using Microsoft.Education.Data;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EDUGraphAPI.Web.Services
{
    /// <summary>
    /// A service class used to get education data by controllers
    /// </summary>
    public class SchoolsService
    {
        private EducationServiceClient educationServiceClient;
        private ApplicationDbContext dbContext;

        public SchoolsService(EducationServiceClient educationServiceClient, ApplicationDbContext dbContext)
        {
            this.educationServiceClient = educationServiceClient;
            this.dbContext = dbContext;
        }

        public async Task<Assignment[]> GetAssignmentsByClassId(string id)
        {
            var result = await educationServiceClient.GetAssignmentsByClassIdAsync(id);
            foreach (var item in result)
            {
                item.ResourceFiles = Assignment.GetResourcesFiles(item.Resources);
            }
            return result;
        }

        public async Task<Assignment> CreateAssignment(Assignment assignment)
        {
            var result = await educationServiceClient.CreateAssignmentAsync(assignment);
            return result;
        }

        public async Task<EducationAssignmentResource> AddAssignmentResourcesAsync(string classId, string assignmentId,string fileName, string resourceUrl)
        {
            return await educationServiceClient.AddAssignmentResourcesAsync(classId, assignmentId,fileName,resourceUrl);
        }

        public async Task<Assignment> PublishAssignmentAsync(string classId, string assignmentId)
        {
            return await educationServiceClient.PublishAssignmentAsync(classId, assignmentId);
        }

        public async Task<EducationAssignmentResource> AddSubmissionResourceAsync(string classId, string assignmentId,string submissionId, string fileName, string resourceUrl)
        {
            return await educationServiceClient.AddSubmissionResourceAsync(classId, assignmentId, submissionId, fileName, resourceUrl);
        }

        public async Task<EducationAssignmentResource[]> GetAssignmentResourcesAsync(string sectionId, string assignmentId)
        {
            var result = await educationServiceClient.GetAssignmentResourcesAsync(sectionId, assignmentId);
            return result;
        }

        public async Task<Assignment> GetAssignmentByIdAsync(string sectionId, string assignmentId)
        { 
             var result = await educationServiceClient.GetAssignmentAsync(sectionId, assignmentId);
            return result;
        }

        public async Task<Submission[]> GetAssignmentSubmissions(string sectionId, string assignmentId)
        {
            var result = await educationServiceClient.GetAssignmentSubmissionsAsync(sectionId, assignmentId);
            foreach (var item in result)
            {
                if (null != item.SubmittedBy.User && !string.IsNullOrEmpty( item.SubmittedBy.User.Id ))
                {
                    item.SubmittedBy.User.DisplayName = await GetUsername(item.SubmittedBy.User.Id);
                }
                if (!string.IsNullOrEmpty(item.SubmittedDateTime))
                {
                    item.SubmittedDateTime = Convert.ToDateTime(item.SubmittedDateTime).ToShortDateString();
                }
                else
                {
                    item.SubmittedDateTime = "";
                }
                item.Resources = (await educationServiceClient.GetSubmissionResourcesAsync(sectionId, assignmentId, item.Id)).ToList();
            }
            return result;
        }

        public async Task<Submission[]> GetAssignmentSubmissionByUserAsync(string sectionId, string assignmentId,string userId)
        {
            var result = await educationServiceClient.GetAssignmentSubmissionsByUserAsync(sectionId, assignmentId, userId);
            return result;
        }
        private async Task<string>  GetUsername(string userId)
        {
            var client = await AuthenticationHelper.GetGraphServiceClientAsync(Permissions.Application);
            var user = await client.Users[userId].Request().GetAsync();
            return user.DisplayName;

        }

        /// <summary>
        /// Get SchoolsViewModel
        /// </summary>
        public async Task<SchoolsViewModel> GetSchoolsViewModelAsync(UserContext userContext)
        {
            var currentUser = userContext.IsStudent
                ? await educationServiceClient.GetStudentAsync() as SectionUser
                : await educationServiceClient.GetTeacherAsync() as SectionUser;

            var schools = (await educationServiceClient.GetSchoolsAsync())
                .OrderBy(i => i.Name)
                .ToArray();
            for (var i = 0; i < schools.Count(); i++)
            {
                var address = string.Format("{0}/{1}/{2}", schools[i].State, HttpUtility.HtmlEncode(schools[i].City), HttpUtility.HtmlEncode(schools[i].Address));
                if (string.IsNullOrEmpty(schools[i].Address) && string.IsNullOrEmpty(schools[i].Zip))
                {
                    schools[i].Address = "-";
                }
            }

            var mySchools = schools
                .Where(i => i.SchoolNumber == currentUser.SchoolId)
                .ToArray();

            var myFirstSchool = mySchools.FirstOrDefault();
            var grade = userContext.IsStudent ? currentUser.EducationGrade : myFirstSchool?.EducationGrade;

            var sortedSchools = mySchools
                .Union(schools.Except(mySchools));
            return new SchoolsViewModel(sortedSchools)
            {
                IsStudent = userContext.IsStudent,
                UserId = currentUser.UserId,
                EducationGrade = grade,
                UserDisplayName = currentUser.DisplayName,
                MySchoolId = currentUser.SchoolId
            };
        }

        /// <summary>
        /// Get SectionsViewModel of the specified school
        /// </summary>
        public async Task<SectionsViewModel> GetSectionsViewModelAsync(UserContext userContext, string objectId, int top)
        {
            var school = await educationServiceClient.GetSchoolAsync(objectId);
            var mySections = await educationServiceClient.GetMySectionsAsync(school.SchoolNumber);
            mySections = mySections.OrderBy(c => c.CombinedCourseNumber).ToArray();
            var allSections = await educationServiceClient.GetAllSectionsAsync(school.SchoolNumber, top, null);
            return new SectionsViewModel(userContext, school, allSections, mySections);
        }

        /// <summary>
        /// Get SectionsViewModel of the specified school
        /// </summary>
        public async Task<SectionsViewModel> GetSectionsViewModelAsync(UserContext userContext, string objectId, int top, string nextLink)
        {
            var school = await educationServiceClient.GetSchoolAsync(objectId);
            var mySections = await educationServiceClient.GetMySectionsAsync(school.SchoolNumber);
            var allSections = await educationServiceClient.GetAllSectionsAsync(school.SchoolNumber, top, nextLink);

            return new SectionsViewModel(userContext.UserO365Email, school, allSections, mySections);
        }

        /// <summary>
        /// Get users, teachers and students of the specified school
        /// </summary>
        public async Task<SchoolUsersViewModel> GetSchoolUsersAsync(UserContext userContext, string objectId, int top)
        {
            var school = await educationServiceClient.GetSchoolAsync(objectId);
            var users = await educationServiceClient.GetMembersAsync(objectId, top, null);
            var students = await educationServiceClient.GetStudentsAsync(school.SchoolNumber, top, null);
            var teachers = await educationServiceClient.GetTeachersAsync(school.SchoolNumber, top, null);
            ArrayResult<SectionUser> studentsInMyClasses = null;
            if (userContext.IsFaculty)
            {
                var mySections = await educationServiceClient.GetMySectionsAsync(true);
                studentsInMyClasses = new ArrayResult<SectionUser>();
                List<SectionUser> studentsList = new List<SectionUser>();
                foreach (var item in mySections)
                {
                    if (item.SchoolId == school.SchoolId)
                    {
                        foreach (var user in item.Members)
                        {
                            if (user.ObjectType == "Student" && studentsList.Where(c => c.O365UserId == user.O365UserId).Count() == 0)
                            {
                                studentsList.Add(user);
                            }
                        }
                    }
                }

                studentsInMyClasses.Value = studentsList.ToArray();
            }
            return new SchoolUsersViewModel(userContext, school, users, students, teachers, studentsInMyClasses);
        }

        /// <summary>
        /// Get users of the specified school
        /// </summary>
        public async Task<SchoolUsersViewModel> GetSchoolUsersAsync(string objectId, int top, string nextLink)
        {
            var school = await educationServiceClient.GetSchoolAsync(objectId);
            var users = await educationServiceClient.GetMembersAsync(objectId, top, nextLink);
            return new SchoolUsersViewModel(school, users, null, null);
        }

        /// <summary>
        /// Get students of the specified school
        /// </summary>
        public async Task<SchoolUsersViewModel> GetSchoolStudentsAsync(string objectId, int top, string nextLink)
        {
            var school = await educationServiceClient.GetSchoolAsync(objectId);
            var students = await educationServiceClient.GetStudentsAsync(school.SchoolNumber, top, nextLink);
            return new SchoolUsersViewModel(school, null, students, null);
        }

        /// <summary>
        /// Get teachers of the specified school
        /// </summary>
        public async Task<SchoolUsersViewModel> GetSchoolTeachersAsync(string objectId, int top, string nextLink)
        {
            var school = await educationServiceClient.GetSchoolAsync(objectId);
            var teachers = await educationServiceClient.GetTeachersAsync(school.SchoolNumber, top, nextLink);
            return new SchoolUsersViewModel(school, null, null, teachers);
        }

        /// <summary>
        /// Get SectionDetailsViewModel of the specified section
        /// </summary>
        public async Task<SectionDetailsViewModel> GetSectionDetailsViewModelAsync(string schoolId, string classId, IGroupRequestBuilder group)
        {
            var school = await educationServiceClient.GetSchoolAsync(schoolId);
            var section = await educationServiceClient.GetSectionAsync(classId);
            var driveRootFolder = await group.Drive.Root.Request().GetAsync();
            
            var schoolTeachers = await educationServiceClient.GetAllTeachersAsync(school.SchoolNumber);
            foreach(var sectionTeacher in section.Teachers)
            {
                schoolTeachers = schoolTeachers.Where( t=> t.O365UserId!= sectionTeacher.O365UserId).ToArray();
            }

            foreach (var user in section.Students)
            {
                var seat = dbContext.ClassroomSeatingArrangements.Where(c => c.O365UserId == user.O365UserId && c.ClassId == classId).FirstOrDefault();
                user.Position = seat?.Position ?? 0;
                var userInDB = dbContext.Users.Where(c => c.O365UserId == user.O365UserId).FirstOrDefault();
                user.FavoriteColor = userInDB == null ? "" : userInDB.FavoriteColor;
            }
            return new SectionDetailsViewModel
            {
                School = school,
                Section = section,
                Conversations = await group.Conversations.Request().GetAllAsync(),
                SeeMoreConversationsUrl = string.Format(Constants.O365GroupConversationsUrl, section.Email),
                DriveItems = await group.Drive.Root.Children.Request().GetAllAsync(),
                SeeMoreFilesUrl = driveRootFolder.WebUrl,
                SchoolTeachers = schoolTeachers
            };
        }

        /// <summary>
        /// Get my classes
        /// </summary>
        public async Task<string[]> GetMyClassesAsync()
        {
            var myClasses = await educationServiceClient.GetMySectionsAsync();
            return myClasses
                .Select(i => i.SectionName)
                .ToArray();
        }
    }
}