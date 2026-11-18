namespace ALKsPersonalUI.Tests.TestInfrastructure;

public abstract class PersonalUiTestContext : BunitContext
{
    protected PersonalUiTestContext()
    {
        Services.AddPersonalUILibrary();
    }
}

