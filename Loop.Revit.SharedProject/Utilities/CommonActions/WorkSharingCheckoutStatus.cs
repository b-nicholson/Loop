using Autodesk.Revit.DB;

namespace Loop.Revit.Utilities.CommonActions
{
    public static class WorkSharingCheckoutStatus
    {
        public static CheckoutResult Check(Document doc, Element element)
        {
            var result = new CheckoutResult();
            if (!doc.IsWorkshared)
            {
                result.IsEditableByUser = true;
                return result;
            }

            var id = element.Id;
            var checkoutStatus = WorksharingUtils.GetCheckoutStatus(doc, id, out string owner);

            if (checkoutStatus == CheckoutStatus.OwnedByOtherUser)
            {
                result.Owner = owner;
                result.IsEditableByUser = false;
            }
            else
            {
                result.IsEditableByUser = true;
            }

            return result;
        }
    }
}
