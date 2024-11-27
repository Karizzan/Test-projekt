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
    [ExpectedException(typeof(ArgumentException))]
    public void TestAtKodenSmiderEnException()
    {
        Patient patient1 = service.GetPatienter().First();
        Laegemiddel laegemiddel = service.GetLaegemidler().First();

        //Test at koden smider en exception på OpretDagligFast
        service.OpretDagligFast(-1, laegemiddel.LaegemiddelId,
            -1, -1, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        service.OpretDagligFast(patient1.PatientId, laegemiddel.LaegemiddelId,
            -1, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        service.OpretDagligFast(patient1.PatientId, laegemiddel.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now.AddDays(3), DateTime.Now);

        service.OpretDagligSkaev(-1, laegemiddel.LaegemiddelId,
            new Dosis[] { new Dosis(DateTime.Now, 3), new Dosis(DateTime.Now.AddHours(6), 2) },
            DateTime.Now, DateTime.Now.AddDays(3));

        service.OpretDagligSkaev(patient1.PatientId, laegemiddel.LaegemiddelId,
            new Dosis[] { },
            DateTime.Now, DateTime.Now.AddDays(3));

        service.OpretDagligSkaev(patient1.PatientId, laegemiddel.LaegemiddelId,
            new Dosis[] { new Dosis(DateTime.Now, 3), new Dosis(DateTime.Now.AddHours(6), 2) },
             DateTime.Now.AddDays(3), DateTime.Now);

        service.OpretPN(-1, laegemiddel.LaegemiddelId, -1, DateTime.Now, DateTime.Now.AddDays(3));

        service.OpretPN(patient1.PatientId, laegemiddel.LaegemiddelId, -1, DateTime.Now, DateTime.Now.AddDays(3));

        service.OpretPN(patient1.PatientId, laegemiddel.LaegemiddelId, 1, DateTime.Now.AddDays(3), DateTime.Now);

        Console.WriteLine("Her kommer der ikke en exception. Testen fejler.");
    }

    [TestMethod]
    public void OpretDagligSkaev()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        Assert.AreEqual(1, service.GetDagligSkæve().Count());
        DagligSkæv test = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId,
new Dosis[] { new Dosis(DateTime.Now, 3), new Dosis(DateTime.Now.AddHours(6), 2) },
            DateTime.Now, DateTime.Now.AddDays(3));


        Assert.AreEqual(2, service.GetDagligSkæve().Count());
        Assert.AreEqual(20, test.samletDosis());
        Assert.AreEqual(5, test.doegnDosis());

    }

    [TestMethod]
    public void OpretPN()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        Assert.AreEqual(4, service.GetPNs().Count());
        PN test = service.OpretPN(patient.PatientId, lm.LaegemiddelId, 3, DateTime.Now, DateTime.Now.AddDays(3));

        Dato dato1 = new Dato();
        dato1.dato = DateTime.Now;

        test.givDosis(dato1);
        Assert.AreEqual(5, service.GetPNs().Count());
        Assert.AreEqual(3, test.samletDosis());
        Assert.AreEqual(3, test.doegnDosis());

    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC1()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DagligFast test = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            -1, 0, 0, 0, DateTime.Now, DateTime.Now.AddDays(3));
        
    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC2()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DagligFast test = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            0, -1, 0, 0,DateTime.Now.AddDays(3), DateTime.Now );
        
    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC3()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DagligFast test = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            0, 0, -1, 0, DateTime.Now, DateTime.Now);
        
    }



    }