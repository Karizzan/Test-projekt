namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;

[TestClass]
public class ServiceTest
{
    private DataService service;

    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
    }

    [TestMethod]
    public void PatientsExist()
    {
        Assert.IsNotNull(service.GetPatienter());
    }

    [TestMethod]
    public void OpretDagligFast()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligFaste().Count());

        DagligFast test = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligFaste().Count());

        Assert.AreEqual(20, test.samletDosis());

        Assert.AreEqual(5, test.doegnDosis());
    }

    [TestMethod]
    public void OpretDagligSkæv()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        Dosis doser = new Dosis();

        Assert.AreEqual(1, service.GetDagligSkæve().Count());

        DagligFast test = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId,
           doser = , DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligFaste().Count());

        Assert.AreEqual(20, test.samletDosis());

        Assert.AreEqual(5, test.doegnDosis());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestAtKodenSmiderEnException()
    {

        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();


        service.OpretDagligFast(-2, lm.LaegemiddelId,
            4, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        // Hvis koden _ikke_ smider en exception,
        // så fejler testen.

        Console.WriteLine("Her kommer der ikke en exception. Testen fejler.");
    }
}