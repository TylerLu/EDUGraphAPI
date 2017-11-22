﻿/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using Microsoft.Education;
using System.Collections.Generic;
using System.Linq;

namespace EDUGraphAPI.Web.ViewModels
{
    public class SchoolsViewModel
    {
        public SchoolsViewModel(IEnumerable<EducationSchool> schools)
        {
            Schools = schools.ToList();
        }

        public SchoolsViewModel()
            : this(Enumerable.Empty<EducationSchool>()) { }

        public List<EducationSchool> Schools { get; set; }

        public bool IsStudent { get; set; }

        public string UserId { get; set; }

        public string EducationGrade { get; set; }

        public string UserDisplayName { get; set; }

        public string MySchoolId { get; set; }

        public bool AreAccountsLinked { get; set; }

        public bool IsLocalAccount { get; set; }
    }
}