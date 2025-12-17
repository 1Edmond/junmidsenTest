using System.Text;

namespace junmidsenTest.FirstTask.Tasks;

public class StringPackingTask
{

    public string Compress(string? input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (!IsValidInput(input))
            throw new ArgumentException("The string must contain only lowercase letters.", nameof(input));

        if (input.Length == 0)
            return string.Empty;

        var result = new StringBuilder();
        int i = 0;

        while (i < input.Length)
        {
            char currentChar = input[i];
            int count = 1;
            while (i + count < input.Length && input[i + count] == currentChar)
            {
                count++;
            }

            result.Append(currentChar);

            if (count > 1)
            {
                result.Append(count);
            }

            i += count;
        }

        return result.ToString();
    }

    public string Decompress(string? compressed)
    {
        ArgumentNullException.ThrowIfNull(compressed);

        if (compressed.Length == 0)
            return string.Empty;

        var result = new StringBuilder();
        int i = 0;

        while (i < compressed.Length)
        {
            if (!char.IsLower(compressed[i]))
                throw new FormatException($"Invalid character at position {i}: ‘{compressed[i]}’. A lowercase letter is expected.");

            char currentChar = compressed[i];
            i++;

            int count = 0;
            while (i < compressed.Length && char.IsDigit(compressed[i]))
            {
                count = count * 10 + (compressed[i] - '0');
                i++;
            }

            if (count == 0)
                count = 1;

            result.Append(currentChar, count);
        }

        return result.ToString();
    }

    private bool IsValidInput(string input) => input.All(c => c >= 'a' && c <= 'z');

}
