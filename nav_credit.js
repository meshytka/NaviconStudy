var Navicon = Navicon || {};

Navicon.nav_credit = (function()
{
    var dateAttributesCheck = function (context)
    {
        let formContext = context.getFormContext();

        // Берем дату начала
        let datestartValue = formContext.getAttribute("nav_datestart").getValue();
        // Берем дату окончания
        let dateendValue = formContext.getAttribute("nav_dateend").getValue();
        // И ее контролл
        let dateendontrol = formContext.getControl("nav_dateend");

        if (datestartValue != null && dateendValue != null)
        {
            if (Date.parse(dateendValue) - Date.parse(datestartValue) <= 31536000000)
            {
                dateendontrol.setNotification("Дата окончания кредитной программы не может быть установлена раньше года с момента даты начала.", "creditEndDate");
            }
            else
            {
                dateendontrol.clearNotification("creditEndDate"); 
            }
        }
    }

    return {

        onLoad : function(context)
        {
            let formContext = context.getFormContext();

            // Берем дату начала
            let datestartAttr = formContext.getAttribute("nav_datestart");
            // Берем дату окончания
            let dateendAttr = formContext.getAttribute("nav_dateend");

            // подписываем проверку разницы дат на их изменение
            datestartAttr.addOnChange( dateAttributesCheck );
            dateendAttr.addOnChange( dateAttributesCheck );
        }
    }
})();