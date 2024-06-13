namespace AcmeSchool.Application.UseCases.Common
{
    public abstract record UseCaseCommand 
    {
        public abstract void ValidateIfFailThrow();
    }
}
