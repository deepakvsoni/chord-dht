namespace Com.Five.Dht.Tests.Service
{
    using Dht.Service;
    using NUnit.Framework;
    using System;
    using FluentAssertions;
    using Dht.Data;
    using Dht.ServiceImpl;

    [TestFixture]
    public class FingerTableTests
    {
        INodeInfo _node, _eightBitNode, _sixteenBitNode, _sixtyFourBitNode;

        [SetUp]
        public void Setup()
        {
            Id id = new Id(new byte[] { 211, 122, 245, 12, 64, 160, 111, 239, 211
            , 231, 225, 143, 185, 85, 104, 185, 11, 132, 160, 151 }, 160);

            Id eightBitId = new Id(new byte[] { 89 }, 8);
            Id sixteenBitId = new Id(new byte[] { 255, 1 }, 16);
            Id sixtyFourBitId = new Id(new byte[] { 255, 1, 0, 0, 0, 0, 0, 0 }
                , 64);

            Uri _uri = new Uri("sock://localhost:5000");

            _node = new NodeInfo
            {
                Id = id,
                Uri = _uri
            };

            _eightBitNode = new NodeInfo
            {
                Id = eightBitId,
                Uri = _uri
            };

            _sixteenBitNode = new NodeInfo
            {
                Id = sixteenBitId,
                Uri = _uri
            };

            _sixtyFourBitNode = new NodeInfo
            {
                Id = sixtyFourBitId,
                Uri = _uri
            };
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_ConstructNullId()
        {
            Action a = () => new FingerTable(null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_Construct()
        {
            Action a = () => new FingerTable(_node);
            a.ShouldNotThrow();
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_GetClosestPredecessor_Null()
        {
            FingerTable table = new FingerTable(_eightBitNode);
            Action a = ()=> table.GetClosestPredecessor(null);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_GetClosestPredecessor()
        {
            FingerTable table = new FingerTable(_eightBitNode);

            Id eightBitId = new Id(new byte[] { 98 }, 8);
            Id eightBitId2 = new Id(new byte[] { 120 }, 8);
            Id eightBitId3 = new Id(new byte[] { 160 }, 8);

            INodeInfo info = new NodeInfo
            {
                Id = eightBitId,
                Uri = new Uri("sock://localhost:5002")
            };
            INodeInfo info2 = new NodeInfo
            {
                Id = eightBitId2,
                Uri = new Uri("sock://localhost:5001")
            };
            INodeInfo info3 = new NodeInfo
            {
                Id = eightBitId3,
                Uri = new Uri("sock://localhost:5003")
            };
            table.AddEntry(info);
            table.AddEntry(info2);
            table.AddEntry(info3);

            INodeInfo predecessor = table.GetClosestPredecessor(new Id(
                new byte[] { 100 }, 8));
            predecessor.Should().NotBeNull();
            predecessor.Id.Should().Be(eightBitId);
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_GetClosestPredecessor_NoEntries()
        {
            FingerTable table = new FingerTable(_eightBitNode);
            Id eightBitId = new Id(new byte[] { 128 }, 8);

            INodeInfo predecessor = table.GetClosestPredecessor(eightBitId);
            predecessor.Id.Should().Equals(_eightBitNode.Id);
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_RemoveEntry_Null()
        {
            FingerTable table = new FingerTable(_eightBitNode);

            Action a = () => table.RemoveEntry(null);
            a.ShouldThrowExactly<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddEntry()
        {
            FingerTable table = new FingerTable(_eightBitNode);

            Id eightBitId = new Id(new byte[] { 64 }, 8);

            INodeInfo info = new NodeInfo
            {
                Id = eightBitId,
                Uri = new Uri("sock://localhost:5001")
            };

            table.AddEntry(info);

            INodeInfo[] nodes = table.GetNodes();
            foreach(INodeInfo node in nodes)
            {
                node.Id.Should().Be(eightBitId);
            }
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddEntry_2()
        {
            FingerTable table = new FingerTable(_eightBitNode);

            Id eightBitId = new Id(new byte[] { 64 }, 8);
            Id eightBitId2 = new Id(new byte[] { 120 }, 8);

            INodeInfo info = new NodeInfo
            {
                Id = eightBitId,
                Uri = new Uri("sock://localhost:5002")
            };
            INodeInfo info2 = new NodeInfo
            {
                Id = eightBitId2,
                Uri = new Uri("sock://localhost:5001")
            };
            table.AddEntry(info);
            table.AddEntry(info2);

            INodeInfo[] nodes = table.GetNodes();
            for(int i = 0; i < 5; ++i)
            {
                nodes[i].Id.Should().Be(eightBitId2);
            }
            nodes[5].Id.Should().Be(eightBitId);
            nodes[6].Id.Should().Be(eightBitId);
            nodes[7].Id.Should().Be(eightBitId);
        }
        
        [Category("Unit")]
        [Test]
        public void FingerTable_AddEntry_3()
        {
            FingerTable table = new FingerTable(_eightBitNode);

            Id eightBitId = new Id(new byte[] { 98 }, 8);
            Id eightBitId2 = new Id(new byte[] { 120 }, 8);
            Id eightBitId3 = new Id(new byte[] { 160 }, 8);

            INodeInfo info = new NodeInfo
            {
                Id = eightBitId,
                Uri = new Uri("sock://localhost:5002")
            };
            INodeInfo info2 = new NodeInfo
            {
                Id = eightBitId2,
                Uri = new Uri("sock://localhost:5001")
            };
            INodeInfo info3 = new NodeInfo
            {
                Id = eightBitId3,
                Uri = new Uri("sock://localhost:5003")
            };
            table.AddEntry(info);
            table.AddEntry(info2);
            table.AddEntry(info3);

            INodeInfo[] nodes = table.GetNodes();
            nodes[0].Id.Should().Be(eightBitId);
            nodes[1].Id.Should().Be(eightBitId);
            nodes[2].Id.Should().Be(eightBitId);
            nodes[3].Id.Should().Be(eightBitId);
            nodes[4].Id.Should().Be(eightBitId2);
            nodes[5].Id.Should().Be(eightBitId3);
            nodes[6].Id.Should().Be(eightBitId3);
            nodes[7].Id.Should().Be(eightBitId);
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddEntry_4()
        {
            _eightBitNode = new NodeInfo
            {
                Id = new Id(new byte[] { 32 }, 8),
                Uri = new Uri("sock://localhost:5000")
            };

            FingerTable table = new FingerTable(_eightBitNode);

            Id eightBitId = new Id(new byte[] { 64 }, 8);
            Id eightBitId2 = new Id(new byte[] { 120 }, 8);

            INodeInfo info = new NodeInfo
            {
                Id = eightBitId,
                Uri = new Uri("sock://localhost:5002")
            };
            INodeInfo info2 = new NodeInfo
            {
                Id = eightBitId2,
                Uri = new Uri("sock://localhost:5001")
            };
            table.AddEntry(info2);
            table.AddEntry(info);

            INodeInfo[] nodes = table.GetNodes();
            for (int i = 0; i < 6; ++i)
            {
                nodes[i].Id.Should().Be(eightBitId);
            }
            nodes[6].Id.Should().Be(eightBitId2);
            nodes[7].Id.Should().Be(eightBitId);
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddEntry_5()
        {
            FingerTable table = new FingerTable(_eightBitNode);

            Id eightBitId = new Id(new byte[] { 64 }, 8);
            Id eightBitId2 = new Id(new byte[] { 120 }, 8);
            Id eightBitId3 = new Id(new byte[] { 32 }, 8);

            INodeInfo info = new NodeInfo
            {
                Id = eightBitId,
                Uri = new Uri("sock://localhost:5002")
            };
            INodeInfo info2 = new NodeInfo
            {
                Id = eightBitId2,
                Uri = new Uri("sock://localhost:5001")
            };
            INodeInfo info3 = new NodeInfo
            {
                Id = eightBitId3,
                Uri = new Uri("sock://localhost:5004")
            };
            table.AddEntry(info2);
            table.AddEntry(info);
            table.AddEntry(info3);

            INodeInfo[] nodes = table.GetNodes();
            for (int i = 0; i < 5; ++i)
            {
                nodes[i].Id.Should().Be(eightBitId2);
            }
            nodes[5].Id.Should().Be(eightBitId3);
            nodes[6].Id.Should().Be(eightBitId3);
            nodes[7].Id.Should().Be(eightBitId3);
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddEntry_6()
        {
            FingerTable table = new FingerTable(_eightBitNode);

            Id _160Id = new Id(new byte[] { 160 }, 8);
            Id _120Id = new Id(new byte[] { 120 }, 8);
            Id _32Id = new Id(new byte[] { 32 }, 8);
            Id _240Id = new Id(new byte[] { 240 }, 8);

            INodeInfo _160Node = new NodeInfo
            {
                Id = _160Id,
                Uri = new Uri("sock://localhost:5002")
            };
            INodeInfo _120Node = new NodeInfo
            {
                Id = _120Id,
                Uri = new Uri("sock://localhost:5001")
            };
            INodeInfo _32Node = new NodeInfo
            {
                Id = _32Id,
                Uri = new Uri("sock://localhost:5004")
            };
            INodeInfo _240Node = new NodeInfo
            {
                Id = _240Id,
                Uri = new Uri("sock://localhost:5005")
            };
            table.AddEntry(_120Node);
            table.AddEntry(_160Node);
            table.AddEntry(_32Node);
            table.AddEntry(_240Node);

            INodeInfo[] nodes = table.GetNodes();
            nodes[0].Id.Should().Be(_120Id);
            nodes[1].Id.Should().Be(_120Id);
            nodes[2].Id.Should().Be(_120Id);
            nodes[3].Id.Should().Be(_120Id);
            nodes[4].Id.Should().Be(_120Id);
            nodes[5].Id.Should().Be(_160Id);
            nodes[6].Id.Should().Be(_160Id);
            nodes[7].Id.Should().Be(_240Id);
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddEntry_Null()
        {
            FingerTable table = new FingerTable(_eightBitNode);

            Action a = () => table.AddEntry(null);
            a.ShouldThrowExactly<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_RemoveEntry()
        {
            FingerTable table = new FingerTable(_eightBitNode);

            Id eightBitId = new Id(new byte[] { 64 }, 8);
            Id eightBitId2 = new Id(new byte[] { 120 }, 8);
            Id eightBitId3 = new Id(new byte[] { 32 }, 8);

            INodeInfo info = new NodeInfo
            {
                Id = eightBitId,
                Uri = new Uri("sock://localhost:5002")
            };
            INodeInfo info2 = new NodeInfo
            {
                Id = eightBitId2,
                Uri = new Uri("sock://localhost:5001")
            };
            INodeInfo info3 = new NodeInfo
            {
                Id = eightBitId3,
                Uri = new Uri("sock://localhost:5004")
            };
            table.AddEntry(info2);
            table.AddEntry(info);
            table.AddEntry(info3);

            table.RemoveEntry(info2);

            INodeInfo[] nodes = table.GetNodes();
            for (int i = 0; i < nodes.Length; ++i)
            {
                nodes[i].Id.Should().Be(eightBitId3);
            }
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_RemoveEntry2()
        {
            _eightBitNode = new NodeInfo
            {
                Id = new Id(new byte[] { 32 }, 8),
                Uri = new Uri("sock://localhost:5000")
            };

            FingerTable table = new FingerTable(_eightBitNode);

            Id eightBitId = new Id(new byte[] { 64 }, 8);
            Id eightBitId2 = new Id(new byte[] { 120 }, 8);
            Id eightBitId3 = new Id(new byte[] { 34 }, 8);

            INodeInfo info = new NodeInfo
            {
                Id = eightBitId,
                Uri = new Uri("sock://localhost:5002")
            };
            INodeInfo info2 = new NodeInfo
            {
                Id = eightBitId2,
                Uri = new Uri("sock://localhost:5001")
            };
            INodeInfo info3 = new NodeInfo
            {
                Id = eightBitId3,
                Uri = new Uri("sock://localhost:5004")
            };
            table.AddEntry(info2);
            table.AddEntry(info);
            table.AddEntry(info3);

            table.RemoveEntry(info2);

            INodeInfo[] nodes = table.GetNodes();
            nodes[0].Id.Should().Be(eightBitId3);
            nodes[1].Id.Should().Be(eightBitId3);
            nodes[2].Id.Should().Be(eightBitId);
            nodes[3].Id.Should().Be(eightBitId);
            nodes[4].Id.Should().Be(eightBitId);
            nodes[5].Id.Should().Be(eightBitId);
            nodes[6].Id.Should().Be(eightBitId3);
            nodes[7].Id.Should().Be(eightBitId3);
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_RemoveEntry3()
        {
            _eightBitNode = new NodeInfo
            {
                Id = new Id(new byte[] { 32 }, 8),
                Uri = new Uri("sock://localhost:5000")
            };

            FingerTable table = new FingerTable(_eightBitNode);

            Id eightBitId = new Id(new byte[] { 64 }, 8);
            Id eightBitId2 = new Id(new byte[] { 120 }, 8);
            Id eightBitId3 = new Id(new byte[] { 34 }, 8);

            INodeInfo info = new NodeInfo
            {
                Id = eightBitId,
                Uri = new Uri("sock://localhost:5002")
            };
            INodeInfo info2 = new NodeInfo
            {
                Id = eightBitId2,
                Uri = new Uri("sock://localhost:5001")
            };
            INodeInfo info3 = new NodeInfo
            {
                Id = eightBitId3,
                Uri = new Uri("sock://localhost:5004")
            };
            table.AddEntry(info2);
            table.AddEntry(info);
            table.AddEntry(info3);

            table.RemoveEntry(info3);

            INodeInfo[] nodes = table.GetNodes();
            nodes[0].Id.Should().Be(eightBitId);
            nodes[1].Id.Should().Be(eightBitId);
            nodes[2].Id.Should().Be(eightBitId);
            nodes[3].Id.Should().Be(eightBitId);
            nodes[4].Id.Should().Be(eightBitId);
            nodes[5].Id.Should().Be(eightBitId);
            nodes[6].Id.Should().Be(eightBitId2);
            nodes[7].Id.Should().Be(eightBitId);
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwo()
        {
            FingerTable table = new FingerTable(_eightBitNode);
            byte[] bytesAfterAddition = table.AddPowerOfTwo(
                new byte[] { 255 }, 7);
            bytesAfterAddition.Should().Equal(new byte[]
            {
                127
            });
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwo_16Bit()
        {
            FingerTable table = new FingerTable(_sixteenBitNode);
            byte[] bytesAfterAddition = table.AddPowerOfTwo(
                new byte[] { 255, 0 }, 8);
            bytesAfterAddition.Should().Equal(new byte[]
            {
                255, 1
            });
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwo_16Bit_2()
        {
            FingerTable table = new FingerTable(_sixteenBitNode);
            byte[] bytesAfterAddition = table.AddPowerOfTwo(
                new byte[] { 255, 1 }, 8);
            bytesAfterAddition.Should().Equal(new byte[]
            {
                255, 2
            });
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwo_64Bit()
        {
            FingerTable table = new FingerTable(_sixtyFourBitNode);
            byte[] bytesAfterAddition = table.AddPowerOfTwo(
                new byte[] { 255, 255, 0, 0, 0, 0, 0, 0 }, 8);
            bytesAfterAddition.Should().Equal(new byte[]
            {
                255, 0, 1, 0,0,0,0,0
            });
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwoNullBytes()
        {
            FingerTable table = new FingerTable(_eightBitNode);
            Action a = () => table.AddPowerOfTwo(
               null, 8);
            a.ShouldThrow<ArgumentNullException>();
        }

        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwoZeroPower()
        {
            FingerTable table = new FingerTable(_eightBitNode);
            Action a = () => table.AddPowerOfTwo(
               new byte[] { 255 }, -1);
            a.ShouldThrow<ArgumentOutOfRangeException>();
        }


        [Category("Unit")]
        [Test]
        public void FingerTable_AddPowerOfTwo24Power()
        {
            FingerTable table = new FingerTable(_eightBitNode);
            Action a = () => table.AddPowerOfTwo(
               new byte[] { 255 }, 24);
            a.ShouldThrow<ArgumentOutOfRangeException>();
        }
    }
}