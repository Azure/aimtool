using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.AzureIntegrationMigration.Tool
{
    /// <summary>
    /// Defines a class that implements the <see cref="IUniqueIdGenerator"/> interface and generates
    /// unique identifiers from a GUID.
    /// </summary>
    public class UniqueIdGenerator : IUniqueIdGenerator
    {
        /// <summary>
        /// This is the maximum allowed base number.
        /// </summary>
        public const int MaximumBaseNumber = 62;

        /// <summary>
        /// This is the minimum allowed base number.
        /// </summary>
        public const int MinimumBaseNumber = 2;

        /// <summary>
        /// This is the default base number.
        /// </summary>
        private const int DefaultBaseNumber = 36;

        /// <summary>
        /// Generates a unique identifier string from a GUID using default value of base 36.
        /// </summary>
        /// <param name="numberBase">Optional base to use when encoding the identifier.</param>
        /// <param name="truncate">Optional number of characters to return.</param>
        /// <returns>The encoded string.</returns>
        public string Generate(int numberBase = DefaultBaseNumber, int truncate = 0)
        {
            // Validate
            if (numberBase < MinimumBaseNumber || numberBase > MaximumBaseNumber)
            {
                throw new ArgumentOutOfRangeException(nameof(numberBase));
            }

            // Generate
            var id = GenerateIdenfitier(numberBase);

            if (truncate > 0)
            {
                id = id.Substring(0, truncate);
            }

            return id;
        }

        /// <summary>
        /// Generates a unique identifier from a GUID.
        /// </summary>
        /// <param name="numberBase">The base to use when encoding the GUID.</param>
        /// <returns>The encoded string.</returns>
        private static string GenerateIdenfitier(int numberBase)
        {
            // Generate a GUID
            var guid = Guid.NewGuid().ToByteArray();

            // Turn each byte array into an unsigned long
            var firstNumber = BitConverter.ToUInt32(guid, 0);
            var secondNumber = BitConverter.ToUInt32(guid, 4);
            var thirdNumber = BitConverter.ToUInt32(guid, 8);
            var fourthNumber = BitConverter.ToUInt32(guid, 12);

            // Convert each to a string using the specified base
            var firstPart = TransformNumberToBaseNString(firstNumber, numberBase);
            var secondPart = TransformNumberToBaseNString(secondNumber, numberBase);
            var thirdPart = TransformNumberToBaseNString(thirdNumber, numberBase);
            var fourthPart = TransformNumberToBaseNString(fourthNumber, numberBase);

            // Return concatenated identifier
            return string.Concat(firstPart, secondPart, thirdPart, fourthPart);
        }

        /// <summary>
        /// Turns the number into an encoded string.
        /// </summary>
        /// <param name="number">The number to encode.</param>
        /// <param name="numberBase">The base to use when encoding the number.</param>
        /// <returns>The encoded string.</returns>
        private static string TransformNumberToBaseNString(uint number, int numberBase)
        {
            // Locals
            var sb = new StringBuilder();
            ulong whole, remainder;
            uint power, lookupIndex;

            // Mapping table
            var mappingTable = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            // Initialise variables
            whole = number;
            power = 1;

            // Loop until we have reduced the whole number (the original number) to zero
            while (whole != 0)
            {
                // Calculate modulus of whole number against base raised to an incrementing power
                remainder = whole % (ulong)Math.Pow(numberBase, power);
                if (remainder != 0)
                {
                    // Remainder might be higher than number base - divide by base raised to power less one
                    // to get actual lookup value.
                    lookupIndex = (uint)(remainder / (ulong)Math.Pow(numberBase, power - 1));

                    // Add encoded value to string
                    sb.Insert(0, mappingTable[lookupIndex]);
                }
                else
                {
                    // Remainder is zero - add encoded value to string (avoids divide by zero exception)
                    sb.Insert(0, mappingTable[0]);
                }

                // Calculate new whole number by subtracting remainder
                whole -= remainder;

                // Increment power for next calculation
                power++;
            }

            // Return the encoded string
            return sb.ToString();
        }
    }
}
