using System;
using NUnit.Framework;

namespace DY.NET.TEST.TEST
{
#if false
    [TestFixture]
    public class OverridingTest
    {
        public class VirtualClass
        {
            public VirtualClass() 
            { 
                Console.WriteLine("VirtualClass - Constructor");
            }

            public virtual void VirtualMethod()
            {
                Console.WriteLine("VirtualClass - VirtualMethod");
            }
        }

        public class VirtualVirtualClass : VirtualClass
        {
            public VirtualVirtualClass() 
            { 
                Console.WriteLine("VirtualVirtualClass - Constructor");
            }

            public virtual void VirtualMethod()
            {
                Console.WriteLine("VirtualVirtualClass - VirtualMethod");
            }
        }

        public class VirtualOverrideClass : VirtualClass
        {
            public VirtualOverrideClass()
            {
                Console.WriteLine("VirtualOverrideClass - Constructor");
            }

            public override void VirtualMethod()
            {
                Console.WriteLine("VirtualOverrideClass - VirtualMethod");
            }
        }

        public class VirtualVirtualOverrdeClass : VirtualVirtualClass
        {
            public VirtualVirtualOverrdeClass() 
            {
                Console.WriteLine("VirtualVirtualOverrdeClass - Constructor");
            }

            public override void VirtualMethod()
            {
                Console.WriteLine("VirtualVirtualOverrdeClass - VirtualMethod");
            }
        }


        public class VirtualOverrideOverrideClass : VirtualOverrideClass
        {
            public VirtualOverrideOverrideClass()
            {
                Console.WriteLine("VirtualOverrideOverrideClass - Constructor");
            }

            public override void VirtualMethod()
            {
                Console.WriteLine("VirtualOverrideOverrideClass - VirtualMethod");
            }
        }

        public class VirtualVirtualNothingClass : VirtualVirtualClass
        {
            public VirtualVirtualNothingClass() 
            {
                Console.WriteLine("VirtualVirtualNothingClass - Constructor");
            }

            public void VirtualMethod()
            {
                Console.WriteLine("VirtualVirtualNothingClass - VirtualMethod");
            }
        }

        public class VirtualVirtualNewClass : VirtualVirtualClass
        {
            public VirtualVirtualNewClass() 
            {
                Console.WriteLine("VirtualVirtualNewClass - Constructor");
            }

            public new void VirtualMethod()
            {
                Console.WriteLine("VirtualVirtualNewClass - VirtualMethod");
            }
        }

        [Test]
        public void VirtualMethodCallTest()
        {
            new VirtualClass().VirtualMethod();
            new VirtualVirtualClass().VirtualMethod();
            new VirtualVirtualOverrdeClass().VirtualMethod();
            new VirtualOverrideClass().VirtualMethod();
            new VirtualOverrideOverrideClass().VirtualMethod();
            new VirtualVirtualNothingClass().VirtualMethod();
            new VirtualVirtualNewClass().VirtualMethod();
        }

        [Test]
        public void VirtualVirtualClassTest()
        {
            VirtualClass vclazz = new VirtualVirtualClass();
            vclazz.VirtualMethod();// VirtualClass-VirtualMethod 호출됨
        }

        [Test]
        public void VirtualVirtualOverrdeClassTest()
        {
            VirtualClass vclazz = new VirtualVirtualOverrdeClass();
            vclazz.VirtualMethod();// VirtualClass-VirtualMethod 호출됨

            VirtualVirtualClass vvclazz = vclazz as VirtualVirtualClass;
            vvclazz.VirtualMethod();// VirtualVirtualOverrdeClass-VirtualMethod 호출됨
        }

        [Test]
        public void VirtualVirtualNothingClassTest()
        {
            VirtualClass vclazz = new VirtualVirtualNothingClass();
            vclazz.VirtualMethod();// VirtualClass-VirtualMethod 호출됨

            VirtualVirtualClass vvclazz = vclazz as VirtualVirtualClass;
            vvclazz.VirtualMethod();// VirtualVirtualClass-VirtualMethod 호출됨
        }

        [Test]
        public void VirtualVirtualNewClassTest()
        {
            VirtualClass vclazz = new VirtualVirtualNewClass();
            vclazz.VirtualMethod();// VirtualClass-VirtualMethod 호출됨

            VirtualVirtualClass vvclazz = vclazz as VirtualVirtualClass;
            vvclazz.VirtualMethod();// VirtualVirtualClass-VirtualMethod 호출됨
        }

        [Test]
        public void VirtualOverrideOverrideClassTest()
        {
            VirtualClass vclazz = new VirtualOverrideOverrideClass();
            vclazz.VirtualMethod();// VirtualOverrideOverrideClass-VirtualMethod 호출됨

            VirtualOverrideClass voclazz = vclazz as VirtualOverrideClass;
            voclazz.VirtualMethod();// VirtualOverrideOverrideClass-VirtualMethod 호출됨
        }
    }
#endif
}