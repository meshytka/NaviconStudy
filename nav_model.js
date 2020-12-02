var Navicon = Navicon || {};

Navicon.nav_model = (function()
{
    return {

        onLoad : function(context)
        {
            let formContext = context.getFormContext();

            let formType = formContext.ui.getFormType();

            if (formType == 2)
            {
                var userSettingsRoles = Xrm.Utility.getGlobalContext().userSettings.roles.getAll();

                console.log(userSettingsRoles);
                var roleName = "System Administrator";

                var isSystemAdmin = false;

                for (var i = 0; i < userSettingsRoles.length; i++) {
                    var userRoleName = userSettingsRoles[i].name;
                    if (userRoleName == roleName) {
                        isSystemAdmin = true;
                        break;
                    }
                }

                if (isSystemAdmin)
                {
                    formContext.ui.controls.forEach(function (control, i) {

                        if (control && control.getDisabled && !control.getDisabled()) 
                        {
                            control.setDisabled(true);
                        }
                    });
                }
            }
        }
    }
})();