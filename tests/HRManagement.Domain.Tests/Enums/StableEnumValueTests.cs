using HRManagement.Domain.Enums;

namespace HRManagement.Domain.Tests.Enums;

public sealed class StableEnumValueTests
{
    [Fact]
    public void PersistedEnums_HaveStableExplicitValues()
    {
        Assert.Equal(1, (int)Gender.Male);
        Assert.Equal(2, (int)Gender.Female);
        Assert.Equal(7, (int)EmploymentStatus.Deceased);
        Assert.Equal(4, (int)MaritalStatus.Widowed);
        Assert.Equal(8, (int)BloodType.ABNegative);
        Assert.Equal(6, (int)MilitaryServiceStatus.NotApplicable);
        Assert.Equal(5, (int)RelationshipType.Other);
        Assert.Equal(8, (int)EducationDegree.Other);
        Assert.Equal(6, (int)ContractType.ProjectBased);
        Assert.Equal(6, (int)TerminationType.Deceased);
        Assert.Equal(3, (int)BackupType.EmergencyBeforeRestore);
        Assert.Equal(4, (int)DependentEducationStatus.UniversityStudent);
        Assert.Equal(3, (int)DependentInsuranceStatus.OtherCoverage);
    }
}
