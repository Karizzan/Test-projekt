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
            0, -1, 0, 0, DateTime.Now.AddDays(3), DateTime.Now);

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

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC4()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DagligFast test = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            0, 0, 0, 0, DateTime.Now, DateTime.Now.AddDays(3));

    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC5()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DagligFast test = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            0, 0, 0, 0, DateTime.Now.AddDays(3), DateTime.Now);

    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC6()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DagligFast test = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            0, 0, 0, 0, DateTime.Now, DateTime.Now);

    }

    [TestMethod]
    public void TC7()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DagligFast test = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            1, 1, 1, 1, DateTime.Now, DateTime.Now.AddDays(3));
        Assert.AreEqual(5, service.GetDagligFaste().Count());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC8()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DagligFast test = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 1, 0, 0, DateTime.Now.AddDays(3), DateTime.Now);
    }

    [TestMethod]
    public void TC9()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DagligFast test = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 1, 0, 0, DateTime.Now, DateTime.Now);
        Assert.AreEqual(6, service.GetDagligFaste().Count());
    }

    [TestMethod]
    public void TC10()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DagligFast test = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            1, 7, 1, 7, DateTime.Now, DateTime.Now.AddDays(3));
        Assert.AreEqual(3, service.GetDagligFaste().Count());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC11()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DagligFast test = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 1, 4, 4, DateTime.Now.AddDays(3), DateTime.Now);
    }

    [TestMethod]
    public void TC12()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DagligFast test = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 1, 4, 4, DateTime.Now, DateTime.Now);
        Assert.AreEqual(4, service.GetDagligFaste().Count());
    }



    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC13()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DagligSkæv test = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId,
        new Dosis[] { new Dosis(DateTime.Now, -1), new Dosis(DateTime.Now.AddHours(6), 0) },
        DateTime.Now, DateTime.Now.AddDays(3));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC14()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DagligSkæv test = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId,
        new Dosis[] { new Dosis(DateTime.Now, -1), new Dosis(DateTime.Now.AddHours(6), 0) },
        DateTime.Now.AddDays(3), DateTime.Now);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC15()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DagligSkæv test = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId,
        new Dosis[] { new Dosis(DateTime.Now, -1), new Dosis(DateTime.Now.AddHours(6), 0) },
        DateTime.Now, DateTime.Now);
    }

    //Fix this test
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC16()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DagligSkæv test = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId,
        new Dosis[] { new Dosis(DateTime.Now, 0), new Dosis(DateTime.Now.AddHours(6), 0) },
        DateTime.Now, DateTime.Now.AddDays(3));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC17()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DagligSkæv test = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId,
        new Dosis[] { new Dosis(DateTime.Now, 0), new Dosis(DateTime.Now.AddHours(6), 0) },
        DateTime.Now.AddDays(3), DateTime.Now);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC18()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DagligSkæv test = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId,
        new Dosis[] { new Dosis(DateTime.Now, -1), new Dosis(DateTime.Now.AddHours(6), 0) },
        DateTime.Now, DateTime.Now);
    }

    [TestMethod]
    public void TC19()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DagligSkæv test = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId,
        new Dosis[] { new Dosis(DateTime.Now, 1), new Dosis(DateTime.Now.AddHours(6), 0) },
        DateTime.Now, DateTime.Now.AddDays(3));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC20()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DagligSkæv test = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId,
        new Dosis[] { new Dosis(DateTime.Now, 1), new Dosis(DateTime.Now.AddHours(6), 0) },
        DateTime.Now.AddDays(3), DateTime.Now);
    }

    [TestMethod]
    public void TC21()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DagligSkæv test = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId,
        new Dosis[] { new Dosis(DateTime.Now, 1), new Dosis(DateTime.Now.AddHours(6), 0) },
        DateTime.Now, DateTime.Now);
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC22()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        PN test = service.OpretPN(patient.PatientId, lm.LaegemiddelId, -1, DateTime.Now, DateTime.Now.AddDays(3));

    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC23()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        PN test = service.OpretPN(patient.PatientId, lm.LaegemiddelId, -1, DateTime.Now.AddDays(3), DateTime.Now);

    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC24()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        PN test = service.OpretPN(patient.PatientId, lm.LaegemiddelId, -1, DateTime.Now, DateTime.Now);

    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC25()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        PN test = service.OpretPN(patient.PatientId, lm.LaegemiddelId, 0, DateTime.Now, DateTime.Now.AddDays(3));

    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC26()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        PN test = service.OpretPN(patient.PatientId, lm.LaegemiddelId, 0, DateTime.Now.AddDays(3), DateTime.Now);

    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC27()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        PN test = service.OpretPN(patient.PatientId, lm.LaegemiddelId, 0, DateTime.Now, DateTime.Now);

    }

    [TestMethod]
    public void TC28()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        PN test = service.OpretPN(patient.PatientId, lm.LaegemiddelId, 1, DateTime.Now, DateTime.Now.AddDays(3));

    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TC29()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        PN test = service.OpretPN(patient.PatientId, lm.LaegemiddelId, 1, DateTime.Now.AddDays(3), DateTime.Now);

    }

    [TestMethod]
    public void TC30()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        PN test = service.OpretPN(patient.PatientId, lm.LaegemiddelId, 1, DateTime.Now, DateTime.Now);

    }



}