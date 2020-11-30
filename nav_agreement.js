var Navicon = Navicon || {};

Navicon.nav_building_ribbon = (function()
{
    var creditProgramVisibleCheck = function (context)
    {
        let formContext = context.getFormContext();

        let autoidAttr = formContext.getAttribute("nav_autoid");
        let contactAttr = formContext.getAttribute("nav_contact");

        var lookup = new Array();
        lookup = formContext.getAttribute("nav_autoid").getValue();
        if (lookup != null)
        {
            console.log(lookup[0].valueOf("id"));
        }

        // есть авто или контакт не выбраны
        if (autoidAttr.getValue() == null || contactAttr.getValue() == null)
        {
            // скрываем кредитную программу
            formContext.getControl("nav_creditid").setVisible(false);
        }
        else
        {
            // показываем кредитную программу
            formContext.getControl("nav_creditid").setVisible(true);
        }
    }

    // Проверка на видимость вкладки "Кредит"
    var creditTabVisibleCheck = function (context)
    {
        let formContext = context.getFormContext();

        let creditProgramAttr = formContext.getAttribute("nav_creditid");
        let tabObj = formContext.ui.tabs.get("Credit");

        if  (creditProgramAttr.getValue() != null)
        {
            tabObj.setVisible(true);
        }
        else
        {
            tabObj.setVisible(false);
        }     
    }

    var addEventHandler = function () 
    {
        // add the event handler for PreSearch Event
        Xrm.Page.getControl("nav_creditid").addPreSearch(function () {
            addFilter();
        });
    }

    var addFilter = function () 
    {
        //var filterAttribute = Xrm.Page.getAttribute("nav_autoid").getAttribute("nav_nav_credit_nav_auto").getValue();
        //create a filter xml
        var filter = "<filter type='and'>" +
                    "<condition attribute='nav_bank' operator='eq' value='" + "Нет" + "'/>" +
                    "</filter>";

        //add filter
        Xrm.Page.getControl("nav_creditid").addCustomFilter(filter);
    }

    var checkNameValue = function (context) 
    {
        let formContext = context.getFormContext();
        let nameAttr = formContext.getAttribute("nav_name");
        let nameValue = nameAttr.getValue();

        if (nameValue != null)
        {
            let regexp = /[^\d-]+/g;

            let newNameValue = nameValue.replace(regexp,"");
            nameAttr.setValue(newNameValue);
        }
    }

    return {

        onLoad : function(context)
        {
            let formContext = context.getFormContext();

            //let formType = formContext.ui.getFormType();

            // Проверяем видимость вкладки по кредиту
            creditTabVisibleCheck(context);

            let nameAttr = formContext.getAttribute("nav_name");
            nameAttr.addOnChange( checkNameValue );
            // Берем контакт
            let contactAttr = formContext.getAttribute("nav_contact");
            // подписываем на проверку видимости кредитной программы
            contactAttr.addOnChange( creditProgramVisibleCheck );

            // берем авто
            let autoidAttr = formContext.getAttribute("nav_autoid");
            // подписываем на проверку видимости кредитной программы
            autoidAttr.addOnChange( creditProgramVisibleCheck );

            // берем сумму
            let summaControl = formContext.getControl("nav_summa");
            // скрываем
            summaControl.setVisible(false);

            // берем оплачен
            let factControl = formContext.getControl("nav_fact");
            //скрываем
            factControl.setVisible(false);

            // берем Кредитную программу
            let creditProgramAttr = formContext.getAttribute("nav_creditid");
            // Завязываем на него видимость вкладки "Кредит"
            creditProgramAttr.addOnChange( creditTabVisibleCheck );
            // Проверяем видимость
            creditProgramVisibleCheck(context);

            // берем Владельца
            let ownerControl = formContext.getControl("ownerid");
            // скрываем
            ownerControl.setVisible(false);

            addEventHandler(context);
        }
    }
})();