using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lynicon.Extensibility;
using Lynicon.Routing;
using Lynicon.Membership;
using Lynicon.Collation;
using Lynicon.Repositories;
using Lynicon.Editors;
using Lynicon.Extensions;
using Lynicon.Models;

namespace Lynicon.Modules
{
    /// <summary>
    /// The core module activates and manages functionality for Lynicon to operate at all.  If not registered, no Lynicon
    /// features will be active, just utility code will be available
    /// </summary>
    public class CoreModule : Module
    {
        public CoreModule(params string[] dependentOn)
            : base("Core", dependentOn)
        {
            if (!VerifyDbState("LyniconInit 0.1"))
            {
                this.Blocked = true;
                return;
            }

            ContentTypeHierarchy.RegisterType(typeof(User));

            Collator.Instance.SetupType(typeof(ContentItem), null, null);
            Collator.Instance.SetupType(typeof(User), new BasicCollator(Repository.Instance), new UserRepository());
        }

        public override void RegisterRoutes(AreaRegistrationContext regContext)
        {
            regContext.MapRoute("lyniconembedded",
                "Lynicon/Embedded/{*innerUrl}",
                new { controller = "Embedded", action = "Index" });
            // Get dynamically generated content
            regContext.MapRoute("lynicondynamic",
                "Lynicon/Dynamic/{action}/{urlTail}",
                new { controller = "Dynamic" });
            regContext.AddDataRoute<List<User>>("lyniconusers",
                "Lynicon/Users",
                new { controller = "User", action = "List", view = "LyniconListDetail", listView = "UserList" },
                new { },
                new { top = "15" }, null);
            regContext.MapRoute("lyniconadmin",
                "Lynicon/{controller}/{action}",
                new { controller = "Ajax", action = "Index" },
                new { controller = "Ajax|Admin|FileManager|Items|Login|Nav|Ui|Upload|UrlManager|Version" }
            );
        }

        public override bool Initialise()
        {
            EditorRedirect.Instance.Register(typeof(List<User>), new ListEditorRedirect(new List<string> { "UserName", "Email", "Created", "Modified", "Roles" }));

            // Set up Url Management
            UrlRequestInterceptor.Register();

            // Set up base UI
            LyniconUi.Instance.FuncPanelButtons.Add(new FuncPanelButton
            {
                Id = "fpbMainLogout",
                Caption = "Log Out",
                DisplayPermission = new ContentPermission("E"),
                Url = "/Lynicon/Login/Logout?returnUrl=$$CurrentUrl$$",
                Section = "Global"
            });
            LyniconUi.Instance.FuncPanelButtons.Add(new FuncPanelButton
            {
                Id = "fpbMainLogin",
                Caption = "Log In",
                DisplayPermission = new ContentPermission { RolesPermitted = roles => !roles.Contains("E") },
                Url = "/Lynicon/Login",
                Section = "Global"
            });
            LyniconUi.Instance.FuncPanelButtons.Add(new FuncPanelButton
            {
                Id = "fpbListItems",
                Caption = "List",
                DisplayPermission = new ContentPermission("E"),
                Url = "/Lynicon/Items",
                Section = "Global"
            });
            LyniconUi.Instance.FuncPanelButtons.Add(new FuncPanelButton
            {
                Id = "fpbFilterItems",
                Caption = "Filtering",
                DisplayPermission = new ContentPermission("E"),
                Url = "/Lynicon/Items/List",
                Section = "Global"
            });
            LyniconUi.Instance.FuncPanelButtons.Add(new FuncPanelButton
            {
                Id = "fpbAdmin",
                Caption = "Admin",
                DisplayPermission = new ContentPermission("A"),
                Url = "/Lynicon/Admin",
                Section = "Global"
            });
            LyniconUi.Instance.FuncPanelButtons.Add(new FuncPanelButton
            {
                Id = "fpbUsers",
                Caption = "Users",
                DisplayPermission = new ContentPermission("A"),
                Url = "/Lynicon/Users?$orderby=Email&$top=15",
                Section = "Global"
            });

            LyniconUi.Instance.FuncPanelButtons.Add(new FuncPanelButton
            {
                Id = "save",
                Caption = "SAVE",
                DisplayPermission = new ContentPermission("E"),
                Section = "Record",
                BackgroundColor = "#cbdfdf"
            });
            LyniconUi.Instance.FuncPanelButtons.Add(new FuncPanelButton
            {
                Id = "fpbMainDelete",
                Caption = "Delete",
                DisplayPermission = new ContentPermission("A"),
                ClientClickScript = @"var $itemIdx = $('#lynicon_itemIndex');
                    var data = ($itemIdx.length > 0 ? { idx: $itemIdx.val() } : {});
                    if (!confirm('Are you sure you want to delete this item?'))
                        return;
                    $.post(""$$BaseUrl$$?$mode=delete$$OriginalQuery$$"",
                    data, function (res) {
                        window.location = window.location.href;
                    });",
                Section = "Record",
                BackgroundColor = "#8c8c8c"
            });

            var modifiedWhenFilter = new DateFieldFilter("Modified When", typeof(IBasicAuditable).GetProperty("Updated"));
            LyniconUi.Instance.Filters.Add(modifiedWhenFilter);
            var modifiedByFilter = new ForeignKeyFilter("Modified By", typeof(User), "UserName", typeof(IBasicAuditable).GetProperty("UserUpdated"));
            LyniconUi.Instance.Filters.Add(modifiedByFilter);
            var createdWhenFilter = new DateFieldFilter("Created When", typeof(IBasicAuditable).GetProperty("Created"));
            LyniconUi.Instance.Filters.Add(createdWhenFilter);
            var createdByFilter = new ForeignKeyFilter("Created By", typeof(User), "UserName", typeof(IBasicAuditable).GetProperty("UserCreated"));
            LyniconUi.Instance.Filters.Add(createdByFilter);

            return true;
        }
    }
}
