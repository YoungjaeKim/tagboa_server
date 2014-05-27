using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication.Models;

namespace WebApplication.Test.Models
{
	[TestClass]
	public class ItemTest
	{
		[TestMethod]
		public void Item_SmokeTest()
		{
			Item item = new Item();

		}

		/* AddTag 메소드 정의
		 * 1. tag가 입력되면 태그를 먼저 검색한다.
		 * 2. 태그가 이미 있으면 무시하고, 없으면 추가한다.
		 * */

		[TestMethod]
		public void AddTagTest_nullTag_throwException()
		{
			// Arrange
			Item item = new Item();
			Tag tag = null;

			try
			{
				// Act
				item.AddTag(tag); // Exception 발생해야 함.
			}
			catch (ArgumentNullException exception)
			{
				Assert.AreEqual("tag", exception.ParamName);
			}
		}

		[TestMethod]
		public void AddTagTest_addNullTag_CountZero()
		{
			// Arrange
			Item item = new Item();
			Tag tag = null;

			// Act
			try
			{
				item.AddTag(tag);
			}
			catch (ArgumentNullException exception)
			{
				// Assert
				Assert.AreEqual(exception.ParamName, "tag");
			}

			// Assert
			Assert.AreEqual(item.Tags.Count, 0, "count should be zero");
		}

		[TestMethod]
		public void AddTagTest_addNullTitleTag_throwException()
		{
			// Arrange
			Item item = new Item();
			Tag tag = new Tag();

			try
			{
				item.AddTag(tag);
			}
			catch (ArgumentNullException exception)
			{
				Assert.AreEqual("tag.Title", exception.ParamName);
			}
		}

		[TestMethod]
		public void AddTagTest_addTag_Count()
		{
			// Arrange
			Item item = new Item();
			Tag tag = new Tag()
			{
				Title = "test"
			};

			// Act
			item.AddTag(tag);

			// Assert
			Assert.AreEqual(item.Tags.Count, 1, "count should be zero");
		}

		public void AddTagTest_addMultipleTags_tagIsAdded()
		{

			Item item = new Item();
			Tag tag1 = new Tag()
			{
				Title = "tag 1"
			};
			Tag tag2 = new Tag()
			{
				Title = "tag 2"
			};
			Tag tag3 = null;

			try
			{
				item.AddTag(tag1);
			}
			catch (ArgumentNullException exception)
			{
				Console.WriteLine(exception.Message);
			}
			catch (Exception)
			{
				Assert.Fail();
			}
			try
			{
				item.AddTag(tag2);
			}
			catch (ArgumentNullException exception)
			{
				Console.WriteLine(exception.Message);
			}
			catch (Exception)
			{
				Assert.Fail();
			}
			try
			{
				item.AddTag(tag3);
			}
			catch (ArgumentNullException exception)
			{
				Console.WriteLine(exception.Message);
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			Assert.AreEqual(item.Tags.Count, 2);
		}
	}
}
