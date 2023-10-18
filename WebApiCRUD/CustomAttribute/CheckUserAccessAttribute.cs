namespace WebApiCRUD.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CheckUserAccessAttribute : Attribute
    {

    }
}
