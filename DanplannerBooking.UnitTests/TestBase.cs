using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace DanplannerBooking.UnitTests;

public abstract class TestBase
{
    protected readonly MockRepository MockRepository;

    protected TestBase()
    {
        MockRepository = new MockRepository(MockBehavior.Strict);
    }

    protected Mock<T> CreateMock<T>() where T : class
        => MockRepository.Create<T>();

    protected void VerifyAll()
        => MockRepository.VerifyAll();
}

