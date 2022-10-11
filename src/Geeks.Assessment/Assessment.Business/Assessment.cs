using System;
using System.Collections.Generic;
using System.Linq;

namespace Assessment.Business
{
    public class Assessment : IAssessment
    {
        /// <summary>
        /// Returns the score with the highest value
        /// </summary>
        public Score? WithMax(IEnumerable<Score> scores)
        {
            return scores?.OrderByDescending(t => t.Value).FirstOrDefault();
        }

        /// <summary>
        /// Returns the average value of the collection. For an empty collection it returns null
        /// </summary>
        public double? GetAverageOrDefault(IEnumerable<int> items)
        {
            if (items == null || items.Count() == 0) return null;
            return items.Average();
        }


        /// <summary>
        /// Appends the suffix value to the text if the text has value. If not, it returns empty.
        /// </summary>
        public string WithSuffix(string text, string suffixValue)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return string.Concat(text, suffixValue);
        }

        /// <summary>
        /// It fetches all the data from the source.
        /// </summary>
        /// <param name="source">The source data provider returns items by page. NextPageToken is the page token of the next page. If there are no more items to return, nextPageToken will be empty. Passing a null or empty string to the provider will return the first page of the data.
        /// If no value is specified for nextPageToken, the provider will return the first page.
        /// </param>
        /// <returns></returns>
        public IEnumerable<Score> GetAllScoresFrom(IDataProvider<Score> source)
        {
            var result = new List<Score>();
            var firstPage = source.GetData(string.Empty);
            if (firstPage.Items != null && firstPage.Items.Any()) result.AddRange(firstPage.Items);
            var nextPageToken = firstPage.NextPageToken;
            while (!string.IsNullOrEmpty(nextPageToken))
            {
                var nextPage = source.GetData(nextPageToken);
                if (nextPage.Items != null && nextPage.Items.Any()) result.AddRange(nextPage.Items);
                nextPageToken = nextPage.NextPageToken;
            }

            return result;
        }

        /// <summary>
        /// Returns child's name prefixed with all its parents' names separated by the specified separator.Example : Parent/Child
        /// </summary>
        public string GetFullName(IHierarchy child, string separator = null)
        {
            separator ??= "/";
            if (child.Parent == null) return $"{child.Name}";
            return GetFullName(child.Parent, separator) + separator + child.Name;
        }

        /// <summary>
        /// Refactor: Returns the value that is closest to the average value of the specified numbers.
        /// </summary>
        public int? ClosestToAverageOrDefault(IEnumerable<int> numbers)
        {
            if (numbers == null || numbers.Count() == 0) return null;

            var result = numbers.ElementAt(0);
            var avg = numbers.Average();
            var diff = Math.Abs(result - avg);

            var orderedNumbers = numbers.OrderBy(t => t);
            for (int i = 1; i < orderedNumbers.Count(); i++)
            {
                var currentDiff = Math.Abs(orderedNumbers.ElementAt(i) - avg);
                if (currentDiff < diff)
                {
                    result = orderedNumbers.ElementAt(i);
                    diff = currentDiff;
                }
                else break;
            }

            return result;
        }

        /// <summary>
        /// Returns date ranges that have similar bookings on each day in the range.
        /// Read the example carefully.
        /// Example : [{Project:HR, Date: 01/02/2020 , Allocation: 10},
        ///            {Project:CRM, Date: 01/02/2020 , Allocation: 15},
        ///            {Project:HR, Date: 02/02/2020 , Allocation: 10},
        ///            {Project:CRM, Date: 02/02/2020 , Allocation: 15},
        /// 
        ///            {Project:HR, Date: 03/02/2020 , Allocation: 15},
        ///            {Project:CRM, Date: 03/02/2020 , Allocation: 15},
        ///            {Project:HR, Date: 04/02/2020 , Allocation: 15},
        ///            {Project:CRM, Date: 04/02/2020 , Allocation: 15},
        /// 
        ///            {Project:HR, Date: 05/02/2020 , Allocation: 15},
        ///            {Project:CRM, Date: 05/02/2020 , Allocation: 15},
        ///            {Project:ECom, Date: 05/02/2020 , Allocation: 15},
        /// 
        ///            {Project:ECom, Date: 06/02/2020 , Allocation: 10},
        ///            {Project:CRM, Date: 06/02/2020 , Allocation: 15}
        ///            {Project:ECom, Date: 07/02/2020 , Allocation: 10},
        ///            {Project:CRM, Date: 07/02/2020 , Allocation: 15}]    
        /// Returns : 
        ///          [
        ///            { From:01/02/2020 , To:02/02/2020 , [{ Project:CRM , Allocation:15 },{ Project:HR , Allocation:10 }]  },
        ///            { From:03/02/2020 , To:04/02/2020 , [{ Project:CRM , Allocation:15 },{ Project:HR , Allocation:15 }]  },
        ///            { From:05/02/2020 , To:05/02/2020 , [{ Project:CRM , Allocation:15 },{ Project:HR , Allocation:15 },{ Project:ECom , Allocation:15 }]  },
        ///            { From:06/02/2020 , To:07/02/2020 , [{ Project:CRM , Allocation:15 },{ Project:ECom , Allocation:10 }]  }
        ///          ]
        /// </summary>
        public IEnumerable<BookingGrouping> Group(IEnumerable<Booking> dates)
        {
            var groupByDate = from booking in dates
                group booking by booking.Date
                into bookingDates
                select new
                {
                    bookingDates.Key,
                    GroupingItems = from b in bookingDates
                        select new BookingGroupingItem
                        {
                            Allocation = b.Allocation,
                            Project = b.Project
                        }
                };
            var result = new List<BookingGrouping>();
            var startDate = groupByDate?.FirstOrDefault()?.Key;
            for (int i = 0; i < groupByDate.Count(); i++)
            {
                if (i + 1 < groupByDate.Count())
                {
                    if (CanBeUnified(groupByDate.ElementAt(i).GroupingItems,
                            groupByDate.ElementAt(i + 1).GroupingItems))
                    {
                        // yield return new BookingGrouping
                        // {
                        //     From = groupByDate.ElementAt(i).Key,
                        //     To = groupByDate.ElementAt(i + 1).Key,
                        //     Items = Unify(groupByDate.ElementAt(i).GroupingItems,
                        //         groupByDate.ElementAt(i + 1).GroupingItems).ToList()
                        // };
                        result.Add(new BookingGrouping
                        {
                            From = groupByDate.ElementAt(i).Key,
                            To = groupByDate.ElementAt(i + 1).Key,
                            Items = Unify(groupByDate.ElementAt(i).GroupingItems,
                                groupByDate.ElementAt(i + 1).GroupingItems).ToList()
                        });
                        i++;
                    }
                    else
                    {
                        // yield return new BookingGrouping
                        // {
                        //     From = groupByDate.ElementAt(i).Key,
                        //     To = groupByDate.ElementAt(i).Key,
                        //     Items = groupByDate.ElementAt(i).GroupingItems.ToList()
                        // };
                        result.Add(new BookingGrouping
                        {
                            From = groupByDate.ElementAt(i).Key,
                            To = groupByDate.ElementAt(i).Key,
                            Items = groupByDate.ElementAt(i).GroupingItems.ToList()
                        });
                    }
                }
                else
                {
                    // yield return new BookingGrouping
                    // {
                    //     From = groupByDate.ElementAt(i).Key,
                    //     To = groupByDate.ElementAt(i).Key,
                    //     Items = groupByDate.ElementAt(i).GroupingItems.ToList()
                    // };
                    result.Add(new BookingGrouping
                    {
                        From = groupByDate.ElementAt(i).Key,
                        To = groupByDate.ElementAt(i).Key,
                        Items = groupByDate.ElementAt(i).GroupingItems.ToList()
                    });
                }
            }

            return result;
        }

        private bool CanBeUnified(IEnumerable<BookingGroupingItem> s1, IEnumerable<BookingGroupingItem> s2)
        {
            // 1: both projects have the same allocation
            var canBeUnified = true;
            foreach (var item in s1)
            {
                if (s2.Any(x => x.Project == item.Project && x.Allocation != item.Allocation))
                {
                    canBeUnified = false;
                    break;
                }
            }

            return canBeUnified;
        }

        private IEnumerable<BookingGroupingItem> Unify(IEnumerable<BookingGroupingItem> s1,
            IEnumerable<BookingGroupingItem> s2)
        {
            var dic = new Dictionary<string, BookingGroupingItem>();
            foreach (var item in s1)
            {
                dic.TryAdd(item.Project, item);
            }

            foreach (var item in s2)
            {
                dic.TryAdd(item.Project, item);
            }

            return dic.Values.ToList();
        }

        /// <summary>
        /// Merges the specified collections so that the n-th element of the second list should appear after the n-th element of the first collection. 
        /// Example : first : 1,3,5 second : 2,4,6 -> result : 1,2,3,4,5,6
        /// </summary>
        public IEnumerable<int> Merge(IEnumerable<int> first, IEnumerable<int> second)
        {
            var result = new List<int>();
            var firstSize = first.Count();
            var secondSize = second.Count();
            for (var i = 0; i < Math.Max(firstSize, secondSize); i++)
            {
                if (i < firstSize) result.Add(first.ElementAt(i));
                if (i < secondSize) result.Add(second.ElementAt(i));
            }

            return result;
        }
    }
}