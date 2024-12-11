using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Simulation
{
    public class TestObjectShouldFollowAndRaise
    {
        [SimulationAttribute(false, 1, 100, true, true, 0.4)]
        public int Value1 { get; set; } = 0;

        [SimulationAttribute(false, 1, 100, true, true, 0.4)]
        public int Value2 { get; set; } = 0;
    }

    public class TestObjectShouldFollowAndFall
    {
        [SimulationAttribute(false, 1, 100, true, false, 0.4)]
        public int Value1 { get; set; } = 0;

        [SimulationAttribute(false, 1, 100, true, false, 0.4)]
        public int Value2 { get; set; } = 0;
    }

    public class TestObjectShouldFollowAndRaiseInMinus
    {
        [SimulationAttribute(false, -100, 0, true, true, 0.4)]
        public int Value1 { get; set; } = 0;

        [SimulationAttribute(false, -100, 0, true, true, 0.4)]
        public int Value2 { get; set; } = 0;
    }

    public class TestObjectShouldFollowAndFallInMinus
    {
        [SimulationAttribute(false, -100, 0, true, false, 0.4)]
        public int Value1 { get; set; } = 0;

        [SimulationAttribute(false, -100, 0, true, false, 0.4)]
        public int Value2 { get; set; } = 0;
    }

    public class TestObjectIntNullable
    {
        [SimulationAttribute(false, 1, 100, true, true, 0.4)]
        public int? Value1 { get; set; } = 0;
    }

    public class TestObjectDoubleNullable
    {
        [SimulationAttribute(false, 1, 100, true, true, 0.4)]
        public double? Value1 { get; set; } = 0.0;
    }

    public class TestObjectLongNullable
    {
        [SimulationAttribute(false, 1, 100, true, true, 0.4)]
        public long? Value1 { get; set; } = 0;
    }

    public class TestObjectIntReversedMinMax
    {
        [SimulationAttribute(false, 100, 1, true, true, 0.4)]
        public int Value1 { get; set; } = 0;
    }


    public class BaseSimulationTests
    {
        [Fact]
        public void TestMultiplePropertiesFilledRaise()
        {
            var testObject = new TestObjectShouldFollowAndRaise();
            var properties = testObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject, property, null);
                }
            }

            Assert.True(testObject.Value1 == 1);
            Assert.True(testObject.Value2 == 1);
        }


        [Fact]
        public void TestMultiplePropertiesFilledFall()
        {
            var testObject = new TestObjectShouldFollowAndFall();
            var properties = testObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject, property, null);
                }
            }

            Assert.True(testObject.Value1 == 100);
            Assert.True(testObject.Value2 == 100);
        }

        [Fact]
        public void TestPropertyRaise()
        {
            var testObject = new TestObjectShouldFollowAndRaise();
            var testObject1 = new TestObjectShouldFollowAndRaise();

            var properties = testObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject, property, null);
                }
            }

            Assert.True(testObject.Value1 == 1);

            var properties1 = testObject1.GetType().GetProperties();
            foreach (var property in properties1)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject1, property, testObject);
                }
            }

            Assert.True(testObject1.Value1 > testObject.Value1);
        }

        [Fact]
        public void TestPropertyFall()
        {
            var testObject = new TestObjectShouldFollowAndFall();
            var testObject1 = new TestObjectShouldFollowAndFall();

            var properties = testObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject, property, null);
                }
            }

            Assert.True(testObject.Value1 == 100);

            var properties1 = testObject1.GetType().GetProperties();
            foreach (var property in properties1)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject1, property, testObject);
                }
            }

            Assert.True(testObject1.Value1 < testObject.Value1);
        }

        [Fact]
        public void TestPropertyRaiseInMinus()
        {
            var testObject = new TestObjectShouldFollowAndRaiseInMinus();
            var testObject1 = new TestObjectShouldFollowAndRaiseInMinus();

            var properties = testObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject, property, null);
                }
            }

            Assert.True(testObject.Value1 == -100);

            var properties1 = testObject1.GetType().GetProperties();
            foreach (var property in properties1)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject1, property, testObject);
                }
            }

            Assert.True(testObject1.Value1 > testObject.Value1);
        }

        [Fact]
        public void TestPropertyFallInMinus()
        {
            var testObject = new TestObjectShouldFollowAndFallInMinus();
            var testObject1 = new TestObjectShouldFollowAndFallInMinus();

            var properties = testObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject, property, null);
                }
            }

            Assert.True(testObject.Value1 == 0);

            var properties1 = testObject1.GetType().GetProperties();
            foreach (var property in properties1)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject1, property, testObject);
                }
            }

            Assert.True(testObject1.Value1 < testObject.Value1);
        }

        [Fact]
        public void CanProcessNullableInt()
        {
            var testObject = new TestObjectIntNullable();
            var properties = testObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject, property, null);
                }
            }

            Assert.True(testObject.Value1 == 1);
        }

        [Fact]
        public void CanProcessNullableDouble()
        {
            var testObject = new TestObjectDoubleNullable();
            var properties = testObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject, property, null);
                }
            }

            Assert.True(testObject.Value1 == 1);
        }

        [Fact]
        public void CanProcessNullableLong()
        {
            var testObject = new TestObjectLongNullable();
            var properties = testObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject, property, null);
                }
            }

            Assert.True(testObject.Value1 == 1);
        }

        [Fact]
        public void CanProcessReversedMinMax()
        {
            var testObject = new TestObjectIntReversedMinMax();
            var properties = testObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                if (simulationAttr != null)
                {
                    BaseSimulator.FillRandomValue(simulationAttr, testObject, property, null);
                }
            }

            Assert.True(testObject.Value1 == 1);
        }
    }
}
