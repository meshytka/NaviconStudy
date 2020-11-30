var Navicon = Navicon || {};

Navicon.nav_auto_ribbon = (function()
{
    var usedAttributesVisibleCheck = function (context)
    {
        let formContext = context.getFormContext();

        let usedAttr = formContext.getAttribute("nav_used");

        if (usedAttr.getValue() == false)
        {
            setAttributesVisibleValue(formContext, false);
        }
        else
        {
            setAttributesVisibleValue(formContext,true);
        }
    }

    // Устанавливаем видимость группы аттрибутов в зависимости от переданного флага
    var setAttributesVisibleValue = function (formContext, value)
    {
        formContext.getControl("nav_km").setVisible(value);
        formContext.getControl("nav_ownerscount").setVisible(value);
        formContext.getControl("nav_isdamaged").setVisible(value);
    }

    return {

        onLoad : function(context)
        {
            let formContext = context.getFormContext();

            // Берем Поле с пробегом
            let usedAttr = formContext.getAttribute("nav_used");
            // подписываем проверку видимости кредитной программы
            usedAttr.addOnChange( UsedAttributesVisibleCheck );

            // Сразу проверяем видимость полей
            usedAttributesVisibleCheck(context);
        }
    }
})();