var Navicon = Navicon || {};

Navicon.nav_agreement = (function()
{
    var autoOnChange = function (context)
    {
        creditProgramVisibleCheck(context);
        setSumma();
    }

    var setSumma = function ()
    {
        let autosArray = Xrm.Page.getAttribute("nav_autoid").getValue();

        let summaAttr = Xrm.Page.getAttribute("nav_summa");

        if (autosArray != null)
        {
            autoRef = autosArray[0];

            var prom = Xrm.WebApi.retrieveRecord("nav_auto", autoRef.id, "?$select=nav_used,nav_amount&$expand=nav_modelid($select=nav_recommendedamount)");
            prom.then(
                function(result)
                {
                    if (result.nav_used == true)
                    {
                        summaAttr.setValue(result.nav_amount);
                    }
                    else
                    {
                        summaAttr.setValue(result.nav_modelid.nav_recommendedamount);
                    }
                },
                function(error)
                {
                    console.error(error.message);
                }
            );
        }
        else
        {
            summaAttr.setValue(null);
        }
    }

    var creditProgramVisibleCheck = function (context)
    {
        let formContext = context.getFormContext();

        let autoidAttr = formContext.getAttribute("nav_autoid");
        let contactAttr = formContext.getAttribute("nav_contact");

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
        var filterAttribute = Xrm.Page.getAttribute("nav_autoid").getValue();

        var fetchXml = [
    "<fetch>",
    "  <entity name='nav_credit'>",
    "   <all-attributes />",
    "    <link-entity name='nav_nav_credit_nav_auto' from='nav_creditid' to='nav_creditid' intersect='true'>",
    "      <link-entity name='nav_auto' from='nav_autoid' to='nav_autoid' intersect='true'>",
    "        <filter>",
    "          <condition attribute='nav_autoid' operator='eq' value='", filterAttribute[0].id, "'/>",
    "        </filter>",
    "      </link-entity>",
    "    </link-entity>",
    "  </entity>",
    "</fetch>",
        ].join("");

        var layoutXml = "<grid name='resultset' jump='nav_name' select='1' icon='1' preview='1'>"

                         + "<row name='result' id='nav_creditid'>"

                           + "<cell name='nav_name' width='300' />"

                           + "<cell name='createdon' width='125' />"

                         + "</row>"

                       + "</grid>";

        var viewName = "Available Text Modules for this Entity";

        var viewId = "{" + "34A611CD-8503-4DE0-8EB7-B16EEAB32EBF" + "}";

        Xrm.Page.getControl("nav_creditid").addCustomView(viewId, "nav_credit", viewName, fetchXml, layoutXml, true);
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

    var creditOnChange = function (context)
    {
        creditTabVisibleCheck(context);
        checkProgramEndDate();
        setCreditPeriodFromCreditProgram();
    }

    var dateOnChange = function ()
    {
        checkProgramEndDate();
    }

    var checkProgramEndDate = function ()
    {
        let creditProgramsArray = Xrm.Page.getAttribute("nav_creditid").getValue();

        if (creditProgramsArray != null)
        {
            creditProgramRef = creditProgramsArray[0];

            var prom = Xrm.WebApi.retrieveRecord("nav_credit", creditProgramRef.id, "?$select=nav_dateend");
            prom.then(
                function(result)
                {
                    var creditProgramDateEnd = result.nav_dateend;
                    checkDatesOfAgreement(creditProgramDateEnd);
                },
                function(error)
                {
                    console.error(error.message);
                }
            );
        }
    }

    var checkDatesOfAgreement = function (creditProgramDateEnd)
    {
        let creditProgramsArray = Xrm.Page.getAttribute("nav_creditid").getValue();
        let creditProgramsСontrol = Xrm.Page.getControl("nav_creditid");

        let dateAgreementValue = Xrm.Page.getAttribute("nav_date").getValue();
        

        if (creditProgramsArray != null && dateAgreementValue != null)
        {
            if (Date.parse(creditProgramDateEnd) < Date.parse(dateAgreementValue))
            {
                creditProgramsСontrol.setNotification("Срок кредитной программы истекает на момент даты начала договора", "agreementBadDate");
            }
            else
            {
                creditProgramsСontrol.clearNotification("agreementBadDate"); 
            }
        }
    }

    var setCreditPeriodFromCreditProgram = function ()
    {
        let creditProgramsArray = Xrm.Page.getAttribute("nav_creditid").getValue();

        if (creditProgramsArray != null)
        {
            creditProgramRef = creditProgramsArray[0];

            var prom = Xrm.WebApi.retrieveRecord("nav_credit", creditProgramRef.id, "?$select=nav_creditperiod");
            prom.then(
                function(result)
                {
                    var creditProgramPeriod = result.nav_creditperiod;
                    
                    let creditPeriod = Xrm.Page.getAttribute("nav_creditperiod");
                    creditPeriod.setValue(creditProgramPeriod);
                },
                function(error)
                {
                    console.error(error.message);
                }
            );
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
            // 
            autoidAttr.addOnChange( autoOnChange );

            // берем сумму
            let summaControl = formContext.getControl("nav_summa");
            // 
            //summaControl.setDisabled(false);

            // берем оплачен
            let factControl = formContext.getControl("nav_fact");
            //скрываем
            factControl.setVisible(false);

            // берем Кредитную программу
            let creditProgramAttr = formContext.getAttribute("nav_creditid");
            // 
            creditProgramAttr.addOnChange( creditOnChange );
            // Проверяем видимость
            creditProgramVisibleCheck(context);

            let dateAttr = formContext.getAttribute("nav_date");
            dateAttr.addOnChange( dateOnChange );
            // берем Владельца
            let ownerControl = formContext.getControl("ownerid");
            // скрываем
            ownerControl.setVisible(false);

            addEventHandler(context);
        }
    }
})();