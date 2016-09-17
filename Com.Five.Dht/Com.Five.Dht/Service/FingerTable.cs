namespace Com.Five.Dht.Service
{
    using Common;
    using Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class FingerTable
    {
        IEnumerable<byte[]> _powers;

        INodeInfo _ownerNode;

        INodeInfo[] _nodes;

        public FingerTable(INodeInfo ownerNode)
        {
            if(null == ownerNode)
            {
                throw new ArgumentNullException(nameof(ownerNode));
            }
            _ownerNode = ownerNode;

            //Get byte[] of powers of two which can be added to the Id.
            _powers = Id.GetPowersOfTwo(ownerNode.Id.MaxNoOfBits);

            _nodes = new INodeInfo[ownerNode.Id.MaxNoOfBits];
        }

        public INodeInfo[] GetNodes()
        {
            return _nodes.ToArray();
        }

        public INodeInfo GetClosestPredecessor(Id nodeId)
        {
            if(null == nodeId)
            {
                throw new ArgumentNullException(nameof(nodeId));
            }
            for (int i = _nodes.Length - 1; i >= 0; --i)
            {
                if(null != _nodes[i] 
                    && _ownerNode.Id <= nodeId
                    && _nodes[i].Id <= nodeId)
                {
                    return _nodes[i];
                }
            }
            return _ownerNode;
        }

        public void AddEntry(INodeInfo nodeInfo)
        {
            if (null == nodeInfo)
            {
                throw new ArgumentNullException(nameof(nodeInfo));
            }

            for (int i =0;i < _nodes.Length; ++i)
            {
                byte[] nextId = AddPowerOfTwo(_ownerNode.Id.Bytes, i);
                /*
                 * If node being added between nextId and current node id, 
                 * with wrap around.
                 */
                if (null == _nodes[i] ||
                    nodeInfo.Id.Bytes.IsBetween(nextId, _nodes[i].Id.Bytes))
                {
                    _nodes[i] = nodeInfo;
                }
            }
        }

        public void RemoveEntry(INodeInfo nodeInfo)
        {
            if (null == nodeInfo)
            {
                throw new ArgumentNullException(nameof(nodeInfo));
            }
            INodeInfo replacementNode = null;

            for (int i = _nodes.Length - 1; i >= 0; --i)
            {
                if(_nodes[i].Id == nodeInfo.Id)
                {
                    break;
                }
                replacementNode = _nodes[i];
            }
            if(null == replacementNode)
            {
                for(int i = 0; i < _nodes.Length; ++i)
                {
                    if(_nodes[i].Id != nodeInfo.Id)
                    {
                        replacementNode = _nodes[i];
                        break;
                    }
                }
            }
            for (int i = 0; i < _nodes.Length; ++i)
            {
                if (_nodes[i].Id == nodeInfo.Id)
                {
                    _nodes[i] = replacementNode;
                }
            }
        }

        /*
         * TODO: Should this be in Id or FingerTable as this 
         * is the only class using it?
         * 
         * Calculates byte[] after adding power of two, this also acts as
         * module 2^maxNoOfBits as the carry is not carried after last byte
         * in array.
         */
        public byte[] AddPowerOfTwo(byte[] bytes, int power)
        {
            if(null == bytes)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            if(0 > power || power >= _ownerNode.Id.MaxNoOfBits)
            {
                throw new ArgumentOutOfRangeException(nameof(power));
            }
            byte[] bytesAfterAddition = new byte[bytes.Length];

            byte[] powerToBeAdded = _powers.ElementAt(power);

            /*
             * Adding from lowest index to high thinking of this as a number
             * split at byte size.
             */
            int carry = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                int sum = (bytes[i] + powerToBeAdded[i] + carry);

                /*
                 * This cast trim the higher bits resulting in correct byte 
                 * value with carry
                 */
                bytesAfterAddition[i] = (byte)sum;

                carry = (sum > 255) ? 1 : 0;
            }

            return bytesAfterAddition;
        }
    }
}