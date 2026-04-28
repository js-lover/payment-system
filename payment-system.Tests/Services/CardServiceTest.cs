using Xunit;
using Moq;
using payment_system.Application.Services.Interfaces;
using payment_system.Application.Services.Implementations;
using payment_system.Application.Repositories;
using payment_system.Application.DTOs.Card;
using payment_system.Domain.Entities;
using payment_system.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace payment_system.Tests
{
    public class CardServiceTest
    {
        private readonly Mock<ICardRepository> _mockCardRepository;
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly CardService _cardService;
        private readonly Mock<AutoMapper.IMapper> _mockMapper;
        private readonly Mock<ILogger<CardService>> _mockLogger;

        public CardServiceTest()
        {
            _mockCardRepository = new Mock<ICardRepository>();
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockMapper = new Mock<AutoMapper.IMapper>();
            _mockLogger = new Mock<ILogger<CardService>>();

            _cardService = new CardService(
                _mockCardRepository.Object,
                _mockAccountRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }


        /// <summary>
        /// with a valid ID it returns success
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetCardByIdAsync_ValidId_ReturnsSuccess()
        {
            //Arrange

            var cardId = Guid.NewGuid();

            var fakeCard = new Card
            {
                Id = cardId,
                CardNumber = "1234567890123456",
                CardName = "Ahmet Yılmaz",
                ExpirationDate = "12/26",
                AccountId = Guid.NewGuid(),
                Status = CardStatus.Active

            };

            var fakeCardDto = new CardDto
            {
                Id = cardId,
                CardNumber = "1234 **** **** 3456",
                CardName = "Ahmet Yılmaz",
                ExpirationDate = "12/26",
                Status = CardStatus.Active
            };

            // when we call with this id it returns the fakecard
            _mockCardRepository
                .Setup(x => x.GetByIdAsync(cardId))
                .ReturnsAsync(fakeCard);

            // mapper returns the fakecarddto in Card -> CardDto transformation
            _mockMapper
                .Setup(x => x.Map<CardDto>(fakeCard))
                .Returns(fakeCardDto);

            // Act - Runs the method we test
            var result = await _cardService.GetCardByIdAsync(cardId);

            //Assert - Validation of the result
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal("1234 **** **** 3456", result.Data.CardNumber);

            //varifies that if we really called the repo?
            _mockCardRepository.Verify(x => x.GetByIdAsync(cardId), Times.Once);
        }


        [Fact]
        public async Task GetCardByIdAsync_EmptyGuid_ReturnsBadRequest()
        {
            // Act
            var result = await _cardService.GetCardByIdAsync(Guid.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);

            // Validation - Because there is a validaton error, we never go to repository
            _mockCardRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }
    }

}