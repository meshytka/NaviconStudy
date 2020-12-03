var Navicon = Navicon || {};

Navicon.nav_agreement_ribbon = (function()
{

    var recalculateCreditamount = function ()
    {
        let creditamountAttr = Xrm.Page.getAttribute("nav_creditamount");

        let summaAttrValue = Xrm.Page.getAttribute("nav_summa").getValue();
        let initialfeeAttrValue = Xrm.Page.getAttribute("nav_initialfee").getValue();

        if (summaAttrValue != null && initialfeeAttrValue != null)
        {
            creditamountAttr.setValue(summaAttrValue - initialfeeAttrValue);
        }
        else
        {
            creditamountAttr.setValue(null);
        }

        recalculateFullcreditamount();
    }

    var recalculateFullcreditamount = function ()
    {
        let fullcreditamountAttr = Xrm.Page.getAttribute("nav_fullcreditamount");

        let creditperiodAttrValue = Xrm.Page.getAttribute("nav_creditperiod").getValue();
        let creditamountAttrValue = Xrm.Page.getAttribute("nav_creditamount").getValue();

        let creditProgramsArray = Xrm.Page.getAttribute("nav_creditid").getValue();

        if (creditProgramsArray != null && creditperiodAttrValue != null && creditamountAttrValue != null)
        {
            creditProgramRef = creditProgramsArray[0];

            var prom = Xrm.WebApi.retrieveRecord("nav_credit", creditProgramRef.id, "?$select=nav_percent");
            prom.then(
                function(result)
                {
                    var creditProgramPercent = result.nav_percent;
                    
                    var newFullcredinamountValue = creditProgramPercent / 100 * creditperiodAttrValue * creditamountAttrValue + creditamountAttrValue;
                    fullcreditamountAttr.setValue(newFullcredinamountValue);
                },
                function(error)
                {
                    console.error(error.message);
                }
            );
        }
        else
        {
            fullcreditamountAttr.setValue(null);
        }
    }

    return {

        RecalculateCredit : function()
        {
            recalculateCreditamount();
        }
    }
})();