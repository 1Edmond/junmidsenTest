using junmidsenTest.FirstTask.Tasks;

namespace junmidsenTest.Test
{
    public class StringPackingTaskTests
    {
        private readonly StringPackingTask _encoder = new ();

        [Fact]
        public void TestCompressionBasique()
        {
            string input = "aaa";
            string expected = "a3";

            string result = _encoder.Compress(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompressionExemple()
        {
            string input = "aaabbcccdde";
            string expected = "a3b2c3d2e";

            string result = _encoder.Compress(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompressionSansRepetition()
        {
            string input = "abcdef";
            string expected = "abcdef";

            string result = _encoder.Compress(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompressionUnSeulCaractere()
        {
            string input = "a";
            string expected = "a";

            string result = _encoder.Compress(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompressionChaineVide()
        {
            string input = "";
            string expected = "";

            string result = _encoder.Compress(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompressionLongueSequence()
        {
            string input = new('z', 99);
            string expected = "z99";

            string result = _encoder.Compress(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestDecompressionBasique()
        {
            string input = "a3";
            string expected = "aaa";

            string result = _encoder.Decompress(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestDecompressionExemple()
        {
            string input = "a3b2c3d2e";
            string expected = "aaabbcccdde";

            string result = _encoder.Decompress(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestDecompressionSansNombre()
        {
            string input = "abcdef";
            string expected = "abcdef";

            string result = _encoder.Decompress(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestDecompressionChaineVide()
        {
            string input = "";
            string expected = "";

            string result = _encoder.Decompress(input);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestCompressionDecompressionSymetrique()
        {
            string original = "aaabbbcccaaabbbccc";

            string compressed = _encoder.Compress(original);
            string decompressed = _encoder.Decompress(compressed);

            Assert.Equal(original, decompressed);
        }

        [Fact]
        public void TestExceptionInputNull_Compress()
        {
            Assert.Throws<ArgumentNullException>(() => _encoder.Compress(null));
        }

        [Fact]
        public void TestExceptionInputNull_Decompress()
        {
            Assert.Throws<ArgumentNullException>(() => _encoder.Decompress(null));
        }

        [Fact]
        public void TestExceptionCaracteresInvalides()
        {
            Assert.Throws<ArgumentException>(() => _encoder.Compress("aaaBBB"));
        }

        [Fact]
        public void TestExceptionDecompressionInvalide()
        {
            Assert.Throws<FormatException>(() => _encoder.Decompress("a3b2X"));
        }
    }
}