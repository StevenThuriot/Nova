#region License
//   
//  Copyright 2013 Steven Thuriot
//   
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//    http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//   
#endregion

using System.Windows;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Extended Size
    /// </summary>
    public struct ExtendedSize
    {
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width { get; private set; }
        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public double Height { get; private set; }

        /// <summary>
        /// Gets the width of the min.
        /// </summary>
        /// <value>
        /// The width of the min.
        /// </value>
        public double MinWidth { get; private set; }
        /// <summary>
        /// Gets the height of the min.
        /// </summary>
        /// <value>
        /// The height of the min.
        /// </value>
        public double MinHeight { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedSize" /> struct.
        /// </summary>
        /// <param name="size">The size.</param>
        public ExtendedSize(Size size)
            : this (size.Width, size.Height)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedSize" /> struct.
        /// </summary>
        /// <param name="length">The length.</param>
        public ExtendedSize(double length)
            : this(length, length)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedSize" /> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="minWidth">Width of the min.</param>
        /// <param name="minHeight">Height of the min.</param>
        public ExtendedSize(double width, double height, double minWidth = 0d, double minHeight = 0d) 
            : this()
        {
            Width = width;
            Height = height;
            MinHeight = minHeight;
            MinWidth = minWidth;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(ExtendedSize other)
        {
            return Width.Equals(other.Width) && Height.Equals(other.Height) && MinWidth.Equals(other.MinWidth) && MinHeight.Equals(other.MinHeight);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ExtendedSize && Equals((ExtendedSize) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Width.GetHashCode();
                hashCode = (hashCode*397) ^ Height.GetHashCode();
                hashCode = (hashCode*397) ^ MinWidth.GetHashCode();
                hashCode = (hashCode*397) ^ MinHeight.GetHashCode();
                return hashCode;
            }
        }
    }
}
