using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DanplannerBooking.Domain.Security;
using Xunit;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_And_VerifyPassword_Roundtrip_Works()
    {
        // Arrange
        var password = "SuperHemmeligt123!";

        // Act
        var hash = PasswordHasher.HashPassword(password);
        var result = PasswordHasher.VerifyPassword(password, hash);

        // Assert
        Assert.True(result);
    }
    [Fact]
    public void HashPassword_SamePassword_GivesDifferentHashes()
    {
        // Arrange
        var password = "SamePassword123!";

        // Act
        var hash1 = PasswordHasher.HashPassword(password);
        var hash2 = PasswordHasher.HashPassword(password);

        // Assert
        Assert.NotEqual(hash1, hash2);   // pga. tilfældig salt
        Assert.True(PasswordHasher.VerifyPassword(password, hash1));
        Assert.True(PasswordHasher.VerifyPassword(password, hash2));
    }
    [Fact]
    public void VerifyPassword_WrongPassword_ReturnsFalse()
    {
        // Arrange
        var correctPassword = "Correct123!";
        var wrongPassword = "Wrong123!";
        var hash = PasswordHasher.HashPassword(correctPassword);

        // Act
        var result = PasswordHasher.VerifyPassword(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public void VerifyPassword_LegacyPlaintext_ActsAsSimpleComparison()
    {
        // Arrange
        var storedPlain = "Legacy123!";
        var correct = "Legacy123!";
        var wrong = "Nope123!";

        // Act
        var ok = PasswordHasher.VerifyPassword(correct, storedPlain);
        var notOk = PasswordHasher.VerifyPassword(wrong, storedPlain);

        // Assert
        Assert.True(ok);
        Assert.False(notOk);
    }
    [Fact]
    public void IsHashed_Recognizes_Hash_From_HashPassword()
    {
        // Arrange
        var password = "Test123!";
        var hash = PasswordHasher.HashPassword(password);

        // Act
        var isHash = PasswordHasher.IsHashed(hash);
        var isPlain = PasswordHasher.IsHashed(password);

        // Assert
        Assert.True(isHash);
        Assert.False(isPlain);
    }
}

