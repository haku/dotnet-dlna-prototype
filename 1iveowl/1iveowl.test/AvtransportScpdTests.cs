using _1iveowl.Models;

namespace _1iveowl.test;

public class AvtransportScpdTests
{
    [Fact]
    public void TestScpd()
    {
        MessagesTests.CheckObjectAndResource(AvtransportScpd.SCPD, "Resources/AvtransportScpd.xml");
    }
}