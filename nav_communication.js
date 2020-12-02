var Navicon = Navicon || {};

Navicon.nav_communication = (function()
{
    var fieldAttributesVisibleCheck = function (context)
    {
        let formContext = context.getFormContext();

        let emailControl = formContext.getControl("nav_email");
        
        let phoneControl = formContext.getControl("nav_phone");

        let typeAttrValue = formContext.getAttribute("nav_type").getValue();

        if (typeAttrValue == null)
        {
            emailControl.setVisible(false);
            phoneControl.setVisible(false);
        }
        else if (typeAttrValue == 808630001)
        {
            phoneControl.setVisible(true);
            emailControl.setVisible(false);
        }
        else if (typeAttrValue == 808630002)
        {
            phoneControl.setVisible(false);
            emailControl.setVisible(true);
        }
    }

    return {

        onLoad : function(context)
        {
            let formContext = context.getFormContext();
            let typeAttr = formContext.getAttribute("nav_type");

            typeAttr.addOnChange( fieldAttributesVisibleCheck );    

            // Сразу проверяем видимость полей
            fieldAttributesVisibleCheck(context);
        }
    }
})();