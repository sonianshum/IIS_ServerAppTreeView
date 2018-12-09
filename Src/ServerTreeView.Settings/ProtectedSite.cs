// --------------------------------------------------------------------------------------------------------------------
/*
* ONE IDENTITY LLC.PROPRIETARY INFORMATION
*
* This software is confidential.One Identity, LLC.or one of its affiliates or  
* subsidiaries, has supplied this software to you under terms of a
* license agreement, nondisclosure agreement or both. 
* 
* You may not copy, disclose, or use this software except in accordance with     
*  those terms.
* 
*
* Copyright 2017 One Identity LLC.
* ALL RIGHTS RESERVED.
*
* ONE IDENTITY LLC.MAKES NO REPRESENTATIONS OR  
* WARRANTIES ABOUT THE SUITABILITY OF THE SOFTWARE,
* EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
* TO THE IMPLIED WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE, OR
* NON-INFRINGEMENT.ONE IDENTITY LLC.SHALL NOT BE
* LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE
* AS A RESULT OF USING, MODIFYING OR DISTRIBUTING
* THIS SOFTWARE OR ITS DERIVATIVES.
*/
// --------------------------------------------------------------------------------------------------------------------

namespace Starling.TwoFactor.HttpModule.Configuration
{
    #region using statements
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    #endregion

    [DataContract]
    public class ProtectedSite
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public bool IsChecked { get; set; }

        [DataMember(Order = 3)]
        public Collection<Application> Applications { get; set; }

    }

    [DataContract]
    public class Application
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string VirtualPath { get; set; }

    }
}
