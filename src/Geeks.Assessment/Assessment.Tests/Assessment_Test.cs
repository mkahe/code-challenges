using System;
using System.Collections.Generic;
using System.Linq;
using Assessment.Business;
using Moq;
using Xunit;

namespace Assessment.Tests
{
    public class AssessmentTest
    {
        private readonly Mock<IDataProvider<Score>> dataProviderMock;
        private readonly Business.Assessment assessment;

        public AssessmentTest()
        {
            dataProviderMock = new Mock<IDataProvider<Score>>();
            assessment = new Business.Assessment();
        }

        public static IEnumerable<object[]> WithMax_Data()
        {
            // scores , expected
            yield return new object[]
            {
                new List<Score>()
                {
                    new Score() {Value = 6},
                    new Score() {Value = 3},
                    new Score() {Value = 7}
                },
                7
            };

            yield return new object[]
            {
                new List<Score>()
                {
                    new Score() {Value = 10},
                    new Score() {Value = 9},
                    new Score() {Value = 0},
                    new Score() {Value = 555}
                },
                555
            };

            yield return new object[]
            {
                new List<Score>()
                {
                },
                null
            };
        }

        [Theory]
        [MemberData(nameof(WithMax_Data))]
        public void WithMax_Test(IEnumerable<Score> scores, int? expected)
        {
            var result = assessment.WithMax(scores);
            Assert.Equal(expected, result?.Value);
        }

        public static IEnumerable<object[]> etAverageOrDefault_Data()
        {
            // items , expected
            yield return new object[] {new List<int> {1, 2, 3, 4, 5}, 3};
            yield return new object[] {new List<int> {2, 3}, 2.5};
            yield return new object[] {new List<int>(), null};
        }

        [Theory]
        [MemberData(nameof(etAverageOrDefault_Data))]
        public void GetAverageOrDefault_Test(IEnumerable<int> items, double? expected)
        {
            var result = assessment.GetAverageOrDefault(items);
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> WithSuffix_Data()
        {
            // text , suffix, expected
            yield return new object[] {"This is a text!", "with suffix", "This is a text!with suffix"};
            yield return new object[] {"AAA", "bbb", "AAAbbb"};
            yield return new object[] {"", "bbb", ""};
        }

        [Theory]
        [MemberData(nameof(WithSuffix_Data))]
        public void WithSuffix_Test(string text, string suffixValue, string expected)
        {
            var result = assessment.WithSuffix(text, suffixValue);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetAllScoresFrom_SeveralPages()
        {
            dataProviderMock.Setup(d => d.GetData(string.Empty))
                .Returns(new DataProvider<Score>.DataProviderResponse<Score>()
                {
                    Items = new[]
                    {
                        new Score() {Value = 1},
                        new Score() {Value = 2},
                        new Score() {Value = 3}
                    },
                    NextPageToken = "1"
                });

            dataProviderMock.Setup(d => d.GetData("1"))
                .Returns(new DataProvider<Score>.DataProviderResponse<Score>()
                {
                    Items = new[]
                    {
                        new Score() {Value = 4},
                        new Score() {Value = 5},
                        new Score() {Value = 6}
                    },
                    NextPageToken = "2"
                });

            dataProviderMock.Setup(d => d.GetData("2"))
                .Returns(new DataProvider<Score>.DataProviderResponse<Score>()
                {
                    Items = new[]
                    {
                        new Score() {Value = 7},
                        new Score() {Value = 8}
                    },
                    NextPageToken = string.Empty
                });

            var result = assessment.GetAllScoresFrom(dataProviderMock.Object);

            Assert.Equal(8, result.Count());
        }

        [Fact]
        public void GetAllScoresFrom_SinglePage()
        {
            dataProviderMock.Setup(d => d.GetData(string.Empty))
                .Returns(new DataProvider<Score>.DataProviderResponse<Score>()
                {
                    Items = new[]
                    {
                        new Score() {Value = 1},
                        new Score() {Value = 2},
                        new Score() {Value = 3}
                    },
                    NextPageToken = string.Empty
                });

            var result = assessment.GetAllScoresFrom(dataProviderMock.Object);

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void GetAllScoresFrom_ZeroPage()
        {
            dataProviderMock.Setup(d => d.GetData(string.Empty))
                .Returns(new DataProvider<Score>.DataProviderResponse<Score>()
                {
                    Items = new Score[] { },
                    NextPageToken = string.Empty
                });

            var result = assessment.GetAllScoresFrom(dataProviderMock.Object);

            Assert.Empty(result);
        }

        public static IEnumerable<object[]> GetFullName_Data()
        {
            yield return new object[]
            {
                new Hierarchy("Sophie", new Hierarchy("Albert", new Hierarchy("Adam", null))), null,
                "Adam/Albert/Sophie"
            };

            yield return new object[]
            {
                new Hierarchy("Sophie", new Hierarchy("Albert", new Hierarchy("Adam", null))), "##",
                "Adam##Albert##Sophie"
            };

            yield return new object[]
            {
                new Hierarchy("Sophie", null), "/", "Sophie"
            };
        }

        [Theory]
        [MemberData(nameof(GetFullName_Data))]
        public void GetFullName_Test(IHierarchy hierachy, string separator, string expected)
        {
            var result = assessment.GetFullName(hierachy, separator);
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> ClosestToAverageOrDefault_Data()
        {
            yield return new object[] {new List<int> {1, 2, 3, 4, 5}, 3};
            yield return new object[] {new List<int> {5, 4, 3, 2, 1}, 3};
            yield return new object[] {new List<int> {10, 3, 7, 2, 8, 8, 1}, 7};
            yield return new object[] {new List<int> {6, 6, 6, 6}, 6};
            yield return new object[] {new List<int> {-4, 2, -10, 0}, -4};
        }

        [Theory]
        [MemberData(nameof(ClosestToAverageOrDefault_Data))]
        public void ClosestToAverageOrDefault_Test(IEnumerable<int> numbers, int? expected)
        {
            var result = assessment.ClosestToAverageOrDefault(numbers);
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> Group_Data()
        {
            yield return new object[]
            {
                new[]
                {
                    new Booking() {Project = "HR", Date = Convert.ToDateTime("01/02/2020"), Allocation = 10},
                    new Booking() {Project = "CRM", Date = Convert.ToDateTime("01/02/2020"), Allocation = 15},
                    new Booking() {Project = "HR", Date = Convert.ToDateTime("02/02/2020"), Allocation = 10},
                    new Booking() {Project = "CRM", Date = Convert.ToDateTime("02/02/2020"), Allocation = 15},

                    new Booking() {Project = "HR", Date = Convert.ToDateTime("03/02/202"), Allocation = 15},
                    new Booking() {Project = "CRM", Date = Convert.ToDateTime("03/02/202"), Allocation = 15},
                    new Booking() {Project = "HR", Date = Convert.ToDateTime("04/02/202"), Allocation = 15},
                    new Booking() {Project = "CRM", Date = Convert.ToDateTime("04/02/202"), Allocation = 15},

                    new Booking() {Project = "HR", Date = Convert.ToDateTime("05/02/202"), Allocation = 15},
                    new Booking() {Project = "CRM", Date = Convert.ToDateTime("05/02/202"), Allocation = 15},
                    new Booking() {Project = "ECom", Date = Convert.ToDateTime("05/02/202"), Allocation = 15},

                    new Booking() {Project = "ECom", Date = Convert.ToDateTime("06/02/202"), Allocation = 10},
                    new Booking() {Project = "CRM", Date = Convert.ToDateTime("06/02/202"), Allocation = 15},
                    new Booking() {Project = "ECom", Date = Convert.ToDateTime("07/02/202"), Allocation = 10},
                    new Booking() {Project = "CRM", Date = Convert.ToDateTime("07/02/202"), Allocation = 15},
                },
                new[]
                {
                    new BookingGrouping
                    {
                        From = Convert.ToDateTime("01/02/2020"), To = Convert.ToDateTime("02/02/2020"),
                        Items = new List<BookingGroupingItem>
                        {
                            new BookingGroupingItem {Project = "HR", Allocation = 10},
                            new BookingGroupingItem {Project = "CRM", Allocation = 15}
                        }
                    },
                    new BookingGrouping
                    {
                        From = Convert.ToDateTime("03/02/2020"), To = Convert.ToDateTime("04/02/2020"),
                        Items = new List<BookingGroupingItem>
                        {
                            new BookingGroupingItem {Project = "HR", Allocation = 15},
                            new BookingGroupingItem {Project = "CRM", Allocation = 15}
                        }
                    },
                    new BookingGrouping
                    {
                        From = Convert.ToDateTime("05/02/2020"), To = Convert.ToDateTime("05/02/202"),
                        Items = new List<BookingGroupingItem>
                        {
                            new BookingGroupingItem {Project = "HR", Allocation = 15},
                            new BookingGroupingItem {Project = "CRM", Allocation = 15},
                            new BookingGroupingItem {Project = "ECom", Allocation = 15},
                        }
                    },
                    new BookingGrouping
                    {
                        From = Convert.ToDateTime("06/02/2020"), To = Convert.ToDateTime("07/02/2020"),
                        Items = new List<BookingGroupingItem>
                        {
                            new BookingGroupingItem {Project = "ECom", Allocation = 10},
                            new BookingGroupingItem {Project = "CRM", Allocation = 15}
                        }
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(Group_Data))]
        public void Group_Test(IEnumerable<Booking> dates, IEnumerable<BookingGrouping> expected)
        {
            var result = assessment.Group(dates);
            Assert.True(expected.Count() == result.Count()
                        && expected.ElementAt(0).Items.Count() == result.ElementAt(0).Items.Count()
                        && expected.ElementAt(1).Items.Count() == result.ElementAt(1).Items.Count()
                        && expected.ElementAt(2).Items.Count() == result.ElementAt(2).Items.Count()
                        && expected.ElementAt(2).Items.Count() == result.ElementAt(2).Items.Count()
            );
        }

        public static IEnumerable<object[]> Merge_Data()
        {
            // first , second , expected
            yield return new object[] {new List<int>(), new List<int>(), new List<int>()};
            yield return new object[]
                {new List<int> {1, 3, 5, 7}, new List<int> {2, 4, 6}, new List<int> {1, 2, 3, 4, 5, 6, 7}};
            yield return new object[]
                {new List<int> {1, 3, 5}, new List<int> {2, 4, 6}, new List<int> {1, 2, 3, 4, 5, 6}};
            yield return new object[] {new int[] {10, 9, 8}, new int[] {0, 1, 2}, new int[] {10, 0, 9, 1, 8, 2}};
            yield return new object[] {new int[] { }, new int[] {1, 2, 3, 4}, new int[] {1, 2, 3, 4}};
        }

        [Theory]
        [MemberData(nameof(Merge_Data))]
        public void Merge_Empty_List_Size(IEnumerable<int> first, IEnumerable<int> second,
            IEnumerable<int> expected)
        {
            var result = assessment.Merge(first, second);
            Assert.True(result.SequenceEqual(expected));
        }
    }
}