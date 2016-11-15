using System;
using System.Xml;
using System.IO;
using System.Collections;

// Originates from http://www.codeproject.com/KB/cs/rssreader.aspx by Codeproject member http://www.codeproject.com/Members/yetanotherchris

/// Version 1.0
namespace Sloppycode.net
{
	#region Event datatype/delegate
	/// <summary>
	/// Holds details about any errors that occured
	/// during the loading or parsing of the RSS feed.
	/// </summary>
	public class RssReaderErrorEventArgs : EventArgs
	{
		/// <summary>
		/// The details of the error.
		/// </summary>
		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		private string message;
	}

	/// <summary>
	/// Represents the method that will handle the RssReader error event.
	/// </summary>
	public delegate void RssReaderErrorEventHandler(object sender, RssReaderErrorEventArgs e);
	#endregion

	#region RssReader class
	/// <summary>
	/// The RssReader class provides a number of static methods for easy
	/// 1 or 2 step retrieval of RSS feeds. RSS feeds can be downloaded from any
	/// URL, and are then parsed into an <see cref="RssFeed">RssFeed</see> data type,
	/// which contains properties representing most aspects of an RSS Feed. A number
	/// of events are available for the calling application to register at the various
	/// stages of the feed request and parsing.
	/// <example>
	/// The following example retrieves the RSS news feed for the BBC news website,
	/// and creates a HTML document from the feed's details. It saves the HTML document
	/// to disk, and launches the default browser with the document. The number of items
	/// displayed is limited to 5. If there is any error, a messagebox is displayed with
	/// the details of the error.
	/// <code>
	/// RssFeed feed = RssReader.GetFeed("http://www.bbc.co.uk/syndication/feeds/news/ukfs_news/front_page/rss091.xml");
	/// 
	/// if ( feed.ErrorMessage == null || feed.ErrorMessage == "" )
	/// {
	///		string template = "&lt;a href=\"%Link%&gt;%Title%&lt;/a&gt;&lt;br/&gt;%Description%&lt;br/&gt;&lt;br/&gt;&lt;ul&gt;%Items%&lt;/ul&gt;";
	///		string itemTemplate = "&lt;li&gt;&lt;a href=\"%Link%&gt;%Title%&lt;/a&gt;&lt;br/&gt;%Description%&lt;/li&gt;";
	/// 	string html = RssReader.CreateHtml(feed,template,itemTemplate,"",5);
	/// 
	/// 	StreamWriter streamWriter = File.CreateText("c:\\rss.html");
	/// 	streamWriter.Write(html);
	/// 	streamWriter.Close();
	/// 
	/// 	System.Diagnostics.Process.Start("c:\\rss.html");
	/// }
	/// else
	/// {
	/// 	MessageBox.Show("Error getting feed:\r\n" +feed.ErrorMessage,"Rss Demo App",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
	/// }
	/// </code>
	/// </example>
	/// </summary>
	public class RssReader
	{
		// Events: XML document loaded, rss element found,
		// channel node found, item parsed, error

		/// <summary>
		/// This event is fired when the feed has finished loading from the URL
		/// provided, into the XML parser.
		/// </summary>
		public event EventHandler FeedLoaded;

		/// <summary>
		/// This event is fired when the root node (typically 'rss') has
		/// been found in the feed.
		/// </summary>
		public event EventHandler RssNodeFound;

		/// <summary>
		/// This event is fired when the channel/child node of the rss node
		/// (typically 'channel') has been found in the feed.
		/// </summary>
		public event EventHandler ChannelNodeFound;

		/// <summary>
		/// This event is fired when an item is added to the <see cref="RssFeed">RssFeed</see>'s
		/// collection of items.
		/// </summary>
		public event EventHandler ItemAdded;

		/// <summary>
		/// This event is fired when an error occurs in the loading or parsing
		/// of the feed. The same error message is also available in the ErrorMessage
		/// property of the <see cref="RssFeed">RssFeed</see> object that is returned
		/// by the <see cref="Retrieve">Retrieve</see> method.
		/// </summary>
		public event RssReaderErrorEventHandler Error;


		/// <summary>
		/// The node name for the channel element
		/// in the RSS feed. This will rarely ever to be
		/// changed. Default is 'channel'.
		/// </summary>
		public string RootNodeName
		{
			get 
			{
				return this.rootNodeName;
			}
			set
			{
				this.rootNodeName = value;
			}
		}

		/// <summary>
		/// The node name for the root rss element
		/// in the RSS feed. This is altered automatically to 'rdf:RDF'
		/// when RdfMode is set to true. Default is 'rss'.
		/// </summary>
		public string ChannelNodeName
		{
			get 
			{
				return this.channelNodeName;
			}
			set
			{
				this.channelNodeName = value;
			}
		}


		/// <summary>
		/// If this is set to true, then the XML document
		/// is parsed slightly different, to cater sites with RDF feeds (such as
		/// slashdot.org and register.com). The whole RDF format is not supported,
		/// but those items in RSS which have a corresponding RDF property, such
		/// as description,title for the channel, and title,description for each
		/// item, are matched.
		/// </summary>
		public bool RdfMode
		{
			get 
			{
				return this.rdfMode;
			}
			set
			{
				if ( value )
				{
					this.rootNodeName = "rdf:RDF";
				}
				else
				{
					this.rootNodeName = "rss";
				}
				this.rdfMode = value;
			}
		}

		/// <summary>
		/// Member for the public property.
		/// </summary>
		private string rootNodeName = "rss";

		/// <summary>
		/// Member for the public property.
		/// </summary>
		private string channelNodeName = "channel";

		/// <summary>
		/// Member for the public property.
		/// </summary>
		private bool rdfMode = false;

		/// <summary>
		/// Retrieves a <see cref="RssFeed">RssFeed</see> object using
		/// the url provided as the source of the Feed.
		/// </summary>
		/// <param name="Url">The url to retrieve the RSS feed from, this can
		/// be in the format of http:// and also file://.. (ftp?)</param>
		/// <param name="RdfFormat">If this is set to true, then the XML document
		/// is parsed slightly different, to cater sites with RDF feeds (such as
		/// slashdot.org and register.com). The whole RDF format is not supported,
		/// but those items in RSS which have a corresponding RDF property, such
		/// as description,title for the channel, and title,description for each
		/// item, are matched.</param>
		/// <returns>A <see cref="RssFeed">RssFeed</see> object from the
		/// RSS feed's details.</returns>
		public static RssFeed GetFeed(string Url,bool RdfFormat)
		{
			RssReader rssReader = new RssReader();
			rssReader.RdfMode = RdfFormat;
			return rssReader.Retrieve(Url);
		}

		/// <summary>
		/// Retrieves a <see cref="RssFeed">RssFeed</see> object using
		/// the url provided as the source of the Feed.
		/// </summary>
		/// <param name="Url">The url to retrieve the RSS feed from, this can
		/// be in the format of http:// and also file://.. (ftp?)</param>
		/// <returns>A <see cref="RssFeed">RssFeed</see> object from the
		/// RSS feed's details.</returns>
		public static RssFeed GetFeed(string Url)
		{
			RssReader rssReader = new RssReader();
			return rssReader.Retrieve(Url);
		}

		/// <summary>
		/// A simplified method of creating a HTML (or any document) from an
		/// RSS Feed. See <see cref="RssHtmlMaker">RssHtmlMaker</see>
		/// </summary>
		/// <param name="Feed">The <see cref="RssFeed">RssFeed</see> object to
		/// get the tokens' data from.</param>
		/// <param name="Template">The overall HTML template (or any other format)
		/// to replace the tokens in.</param>
		/// <param name="ItemPrefix">A string template that is prepended to the beginning
		/// of each RSS item.</param>
		/// <param name="ItemSuffix">A string template that is apppended to the end
		/// of each RSS item.</param>
		/// <returns>A string with the templates provided parsed of their tokens, with
		/// the data values in their place.</returns>
		public static string CreateHtml(RssFeed Feed,string Template,string ItemPrefix,string ItemSuffix)
		{
			return new RssHtmlMaker().GetHtmlContents(Feed,Template,ItemPrefix,ItemSuffix);
		}

		/// <summary>
		/// A simplified method of creating a HTML (or any document) from an
		/// RSS Feed. See <see cref="RssHtmlMaker">RssHtmlMaker</see>
		/// </summary>
		/// <param name="Feed">The <see cref="RssFeed">RssFeed</see> object to
		/// get the tokens' data from.</param>
		/// <param name="Template">The overall HTML template (or any other format)
		/// to replace the tokens in.</param>
		/// <param name="ItemPrefix">A string template that is prepended to the beginning
		/// of each RSS item.</param>
		/// <param name="ItemSuffix">A string template that is apppended to the end
		/// of each RSS item.</param>
		/// <param name="MaxItems">The maximum number of RSS items to display.</param>
		/// <returns>A string with the templates provided parsed of their tokens, with
		/// the data values in their place.</returns>
		public static string CreateHtml(RssFeed Feed,string Template,string ItemPrefix,string ItemSuffix,int MaxItems)
		{
			RssHtmlMaker rssHtmlMaker = new RssHtmlMaker();
			rssHtmlMaker.MaxItems = MaxItems;
			return rssHtmlMaker.GetHtmlContents(Feed,Template,ItemPrefix,ItemSuffix);
		}

		/// <summary>
		/// Retrieves an RSS feed using the given Url, parses it and
		/// creates and new <see cref="RssFeed">RssFeed</see> object with the information.
		/// If an error occurs in the XML loading of the document, or parsing of
		/// the RSS feed, the error is trapped and stored inside the RssFeed's
		/// ErrorMessage property.
		/// </summary>
		/// <param name="Url">The url to retrieve the RSS feed from, this can
		/// be in the format of http:// and also file://.. (ftp?)</param>
		/// <returns>An <see cref="RssFeed">RssFeed</see> object with information
		/// retrieved from the feed.</returns>
		public RssFeed Retrieve(string Url)
		{
			
			RssFeed rssFeed = new RssFeed();
			rssFeed.Items = new RssItems();

			XmlTextReader xmlTextReader = new XmlTextReader(Url);
			XmlValidatingReader xmlValidatingReader = new XmlValidatingReader(xmlTextReader);
			xmlValidatingReader.ValidationType = ValidationType.None;

			XmlDocument xmlDoc= new XmlDocument();
			
			try
			{
				xmlDoc.Load(xmlTextReader);

				// Fire the load event
				if ( this.FeedLoaded != null )
				{
					this.FeedLoaded(this, new EventArgs());
				}

				XmlNode rssXmlNode = null;

				// Loop child nodes till we find the rss one
				for (int i=0;i < xmlDoc.ChildNodes.Count;i++)
				{
					System.Diagnostics.Debug.Write("Child: " +xmlDoc.ChildNodes[i].Name);
					System.Diagnostics.Debug.WriteLine(" has " +xmlDoc.ChildNodes[i].ChildNodes.Count+" children");
					
					if ( xmlDoc.ChildNodes[i].Name == this.rootNodeName && xmlDoc.ChildNodes[i].ChildNodes.Count > 0 )
					{
						rssXmlNode = xmlDoc.ChildNodes[i];

						// Fire the found event
						if ( this.RssNodeFound != null )
						{
							this.RssNodeFound(this,new EventArgs());
						}

						break;
					}
				}

				if ( rssXmlNode != null )
				{
					XmlNode channelXmlNode = null;	

					// Loop through the rss node till we find the channel
					for (int i=0;i < rssXmlNode.ChildNodes.Count;i++)
					{
						System.Diagnostics.Debug.WriteLine("Rss child: "+rssXmlNode.ChildNodes[i].Name);
						if ( rssXmlNode.ChildNodes[i].Name == this.channelNodeName && rssXmlNode.ChildNodes[i].ChildNodes.Count > 0 )
						{
							channelXmlNode = rssXmlNode.ChildNodes[i];

							// Fire the found event
							if ( this.ChannelNodeFound != null )
							{
								this.ChannelNodeFound(this,new EventArgs());
							}

							break;
						}
					}

					// Found the channel node
					if ( channelXmlNode != null )
					{
						// Loop through its children, copying details to the
						// RssFeed struct, and parsing the items
						for (int i=0;i < channelXmlNode.ChildNodes.Count;i++)
						{
							System.Diagnostics.Debug.WriteLine(channelXmlNode.ChildNodes[i].Name);
							switch ( channelXmlNode.ChildNodes[i].Name )
							{
								case "title":
								{
									rssFeed.Title = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "description":
								{
									rssFeed.Description = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "language":
								{
									rssFeed.Language = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "copyright":
								{
									rssFeed.Copyright = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "webmaster":
								{
									rssFeed.Webmaster = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "pubDate":
								{
									rssFeed.PubDate = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "lastBuildDate":
								{
									rssFeed.LastBuildDate = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "category":
								{
									rssFeed.Category = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "generator":
								{
									rssFeed.Generator = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "ttl":
								{
									rssFeed.Ttl = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "rating":
								{
									rssFeed.Rating = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "skipHours":
								{
									rssFeed.Skiphours = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "skipDays":
								{
									rssFeed.Skipdays = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "managingEditor":
								{
									rssFeed.ManagingEditor = channelXmlNode.ChildNodes[i].InnerText;
									break;
								}
								case "item":
								{
									rssFeed.Items.Add( this.getRssItem(channelXmlNode.ChildNodes[i]) );
									
									// Fire the found event
									if ( this.ItemAdded != null )
									{
										this.ItemAdded(this,new EventArgs());
									}

									break;
								}
							}

						}

						// If rdf mode is set, then the channel node only contains
						// information about the channel, it doesn't hold the item
						// nodes. The item nodes are children of the root node in
						// an RDF document, so we use this instead.
						if ( this.RdfMode ) 
						{
							for (int i=0;i < rssXmlNode.ChildNodes.Count;i++)
							{
								switch ( rssXmlNode.ChildNodes[i].Name )
								{
									case "item":
									{
										rssFeed.Items.Add( this.getRssItem(rssXmlNode.ChildNodes[i]) );

										// Fire the found event
										if ( this.ItemAdded != null )
										{
											this.ItemAdded(this,new EventArgs());
										}

										break;
									}
								}
							}
						}
					}
					else
					{
						rssFeed.ErrorMessage = "Unable to find rss <seehannel> node";

						// Fire the error event
						if ( this.Error != null )
						{
							RssReaderErrorEventArgs args = new RssReaderErrorEventArgs();
							args.Message = rssFeed.ErrorMessage;
							this.Error(this,args);
						}
					}
						
				}
				else
				{
					rssFeed.ErrorMessage = "Unable to find root <rss> node";

					// Fire the error event
					if ( this.Error != null )
					{
						RssReaderErrorEventArgs args = new RssReaderErrorEventArgs();
						args.Message = rssFeed.ErrorMessage;
						this.Error(this,args);
					}
				}

			}
			catch (XmlException err)
			{
				//
				rssFeed.ErrorMessage = "Xml error: " +err.Message;

				// Fire the error event
				if ( this.Error != null )
				{
					RssReaderErrorEventArgs args = new RssReaderErrorEventArgs();
					args.Message = rssFeed.ErrorMessage;
					this.Error(this,args);
				}
				return rssFeed;
			}

			return rssFeed;
		}

		/// <summary>
		/// Creates an RSS item from an XML node with the 
		/// corresponding child nodes (title,description etc.)
		/// </summary>
		/// <param name="xmlNode">The node to extract the details from</param>
		/// <returns>An RssItem object with details taken from the item node.</returns>
		private RssItem getRssItem(XmlNode xmlNode)
		{
			RssItem rssItem = new RssItem();

			for (int i=0;i < xmlNode.ChildNodes.Count;i++)
			{					
				switch ( xmlNode.ChildNodes[i].Name )
				{
					case "title":
					{
						rssItem.Title = xmlNode.ChildNodes[i].InnerText;
						break;
					}
					case "description":
					{
						rssItem.Description = xmlNode.ChildNodes[i].InnerText;
						break;
					}
					case "link":
					{
						rssItem.Link = xmlNode.ChildNodes[i].InnerText;
						break;
					}
					case "author":
					{
						rssItem.Author = xmlNode.ChildNodes[i].InnerText;
						break;
					}
					case "comments":
					{
						rssItem.Comments = xmlNode.ChildNodes[i].InnerText;
						break;
					}
					case "pubdate":
					{
						rssItem.Pubdate = xmlNode.ChildNodes[i].InnerText;
						break;
					}
					case "guid":
					{
						rssItem.Guid = xmlNode.ChildNodes[i].InnerText;
						break;
					}
				}
			}

			return rssItem;
		}
	}
	#endregion

	#region Html creator class
	/// <summary>
	/// This class provides an easy method of converting a <see cref="RssFeed">RssFeed</see>
	/// object into a simple HTML document. This document can then be written to
	/// file, where it can be stored in a cached state (saving a feed request each
	/// time the feed is required). 
	/// </summary>
	public class RssHtmlMaker
	{

		/// <summary>
		/// Restricts the number of items that are displayed and replaced
		/// using the %Items% token in the HTML template.
		/// </summary>
		public int MaxItems
		{
			get 
			{
				return this.maxItems;
			}
			set
			{
				this.maxItems = value;
			}
		}

		/// <summary>
		/// Member for the public property.
		/// </summary>
		private int maxItems = 0;

		/// <summary>
		/// Creates a HTML document, or any format - this is only limited by
		/// the template you provide - from the provided
		/// <see cref="RssFeed">RssFeed</see> object. The tokens described in the
		/// remarks section are replaced with their values inside the template.
		/// The items in the RSS feed are replaced using the ItemPrefix and ItemSuffix
		/// templates, where the suffix is placed face, and the suffix is appended on the end.
		/// </summary>
		/// <remarks>
		/// The following are a list of tokens which are replaced inside the main Template,
		/// with their corresponding values from the provided <see cref="RssFeed">RssFeed</see> 
		/// object. For details on each token, see its corresponding property in the 
		/// <see cref="RssFeed">RssFeed</see> object.
		/// <list type="bullet">
		/// <item>%Title%</item>
		/// <item>%Description%</item>
		/// <item>%Link%</item>
		/// <item>%Language%</item>
		/// <item>%Copyright%</item>
		/// <item>%Webmaster%</item>
		/// <item>%PubDate%</item>
		/// <item>%LastBuildDate%</item>
		/// <item>%Category%</item>
		/// <item>%Generator%</item>
		/// <item>%Ttl%</item>
		/// <item>%Rating%</item>
		/// <item>%Skiphours%</item>
		/// <item>%Skipdays%</item>
		/// <item>%Skipdays%</item>
		/// <item>%Items% - This is replaced by the parsed template of the items</item>
		/// </list>
		/// The following are a list of tokens which are replaced inside the ItemPrefix
		/// and ItemSuffix templates, with their corresponding values from the 
		/// provided <see cref="RssItem">RssItem</see> object. For details
		/// on each token, see its corresponding property in 
		/// the <see cref="RssItem">RssItem</see> object.
		/// <list type="bullet">
		/// <item>%Title%</item>
		/// <item>%Description%</item>
		/// <item>%Link%</item>
		/// <item>%Author%</item>
		/// <item>%Comments%</item>
		/// <item>%Pubdate%</item>
		/// <item>%Guid%</item>
		/// </list>
		/// </remarks>
		/// <param name="Feed">The <see cref="RssFeed">RssFeed</see> object to
		/// get the tokens' data from.</param>
		/// <param name="Template">The overall HTML template (or any other format)
		/// to replace the tokens in.</param>
		/// <param name="ItemPrefix">A string template that is prepended to the beginning
		/// of each RSS item.</param>
		/// <param name="ItemSuffix">A string template that is apppended to the end
		/// of each RSS item.</param>
		/// <returns>A string with the templates provided parsed of their tokens, with
		/// the data values in their place.</returns>
		public string GetHtmlContents(RssFeed Feed,string Template,string ItemPrefix,string ItemSuffix)
		{
			string result = Template;

			// Replace all template tokens
			result = result.Replace("%Title%",Feed.Title);
			result = result.Replace("%Description%",Feed.Description);
			result = result.Replace("%Link%",Feed.Link);
			result = result.Replace("%Language%",Feed.Language);
			result = result.Replace("%Copyright%",Feed.Copyright);
			result = result.Replace("%Webmaster%",Feed.Webmaster);
			result = result.Replace("%PubDate%",Feed.PubDate);
			result = result.Replace("%LastBuildDate%",Feed.LastBuildDate);
			result = result.Replace("%Category%",Feed.Category);
			result = result.Replace("%Generator%",Feed.Generator);
			result = result.Replace("%Ttl%",Feed.Ttl);
			result = result.Replace("%Rating%",Feed.Rating);
			result = result.Replace("%Skiphours%",Feed.Skiphours);
			result = result.Replace("%Skipdays%",Feed.Skipdays);
			result = result.Replace("%Skipdays%",Feed.ManagingEditor);

			// Parse item template
			string itemsContent = "";
			string tempContent = "";

			if ( maxItems == 0 || maxItems > Feed.Items.Count )
			{
				maxItems = Feed.Items.Count;
			}

			for (int i=0;i < maxItems;i++)
			{
				// Parse prefix template
				tempContent  = ItemPrefix;
				tempContent  = tempContent.Replace("%Title%",Feed.Items[i].Title);
				tempContent  = tempContent.Replace("%Description%",Feed.Items[i].Description);
				tempContent  = tempContent.Replace("%Link%",Feed.Items[i].Link);
				tempContent  = tempContent.Replace("%Author%",Feed.Items[i].Author);
				tempContent  = tempContent.Replace("%Comments%",Feed.Items[i].Comments);
				tempContent  = tempContent.Replace("%Pubdate%",Feed.Items[i].Pubdate);
				tempContent  = tempContent.Replace("%Guid%",Feed.Items[i].Guid);

				itemsContent += tempContent;

				// Parse suffix template
				tempContent  = ItemSuffix;
				tempContent  = tempContent.Replace("%Title%",Feed.Items[i].Title);
				tempContent  = tempContent.Replace("%Description%",Feed.Items[i].Description);
				tempContent  = tempContent.Replace("%Link%",Feed.Items[i].Link);
				tempContent  = tempContent.Replace("%Author%",Feed.Items[i].Author);
				tempContent  = tempContent.Replace("%Comments%",Feed.Items[i].Comments);
				tempContent  = tempContent.Replace("%Pubdate%",Feed.Items[i].Pubdate);
				tempContent  = tempContent.Replace("%Guid%",Feed.Items[i].Guid);

				itemsContent += tempContent;
			}

			// Replace %items% with items
			result = result.Replace("%Items%",itemsContent);

			return result;
		}
	}
	#endregion

	#region Data structures
	/// <summary>
	/// A data type to represent all properties of single RSS feed.
	/// (one XML document). The descriptions for
	/// the properties of RssItem are para-phrased from the 
	/// <see href="http://blogs.law.harvard.edu/tech/rss">RSS 2 specification</see>.
	/// See <see cref="RssReader">RssReader</see> for properties which 
	/// have not yet been implemented in this version of the
	/// the RssReader class.
	/// </summary>
	/// <remarks>
	/// The following elements of the RSS &lt;channel&gt; node aren't
	/// supported by this version of RssReader:
	/// <list type="bullet">
	/// <item>image (has subelements: image,url,title,link)</item>
	/// <item>cloud (has attributes: domain,port,path,registerProcedure,protocol)</item>
	/// <item>textInput (has subelements: title,description,name,link)</item>
	/// </list>
	/// </remarks>
	[Serializable()]
	public struct RssFeed
	{
		/// <summary>
		/// The name of the channel.
		/// </summary>
		public string Title;
		/// <summary>
		/// Phrase or sentence describing the channel.
		/// </summary>
		public string Description;
		/// <summary>
		/// The URL to the HTML website corresponding to the channel.
		/// </summary>
		public string Link;

		// Optional items

		/// <summary>
		/// The language the channel is written in. This allows 
		/// aggregators to group all Italian language sites, for example, on a single page. 
		/// </summary>
		public string Language;
		/// <summary>
		/// Copyright notice for content in the channel.
		/// </summary>
		public string Copyright;
		/// <summary>
		/// Email address for person responsible for technical issues relating to channel.
		/// </summary>
		public string Webmaster;
		/// <summary>
		/// The publication date for the content in the channel. 
		/// </summary>
		public string PubDate;
		/// <summary>
		/// The last time the content of the channel changed.
		/// </summary>
		public string LastBuildDate;
		/// <summary>
		/// Specify one or more categories that the channel belongs to.
		/// </summary>
		public string Category;
		/// <summary>
		/// A string indicating the program used to generate the channel.
		/// </summary>
		public string Generator;
		/// <summary>
		/// ttl stands for time to live. It's a number of minutes 
		/// that indicates how long a channel can be cached before 
		/// refreshing from the source
		/// </summary>
		public string Ttl;
		/// <summary>
		/// The <see href="http://www.w3.org/PICS/">PICS</see> rating for the channel.
		/// </summary>
		public string Rating;
		/// <summary>
		/// A hint for aggregators telling them which hours they can skip. 
		/// </summary>
		public string Skiphours;
		/// <summary>
		/// A hint for aggregators telling them which days they can skip. 
		/// </summary>
		public string Skipdays;
		/// <summary>
		/// Email address for person responsible for editorial content.
		/// </summary>
		public string ManagingEditor;
		/// <summary>
		/// A collection of RssItem datatypes, representing each
		/// item for the RSS feed.
		/// </summary>
		public RssItems Items;
		/// <summary>
		/// Contains any errors that occured during the loading or
		/// parsing of the XML document. Compare this to a blank string
		/// to see if any errors occured.
		/// </summary>
		public string ErrorMessage;
	}

	/// <summary>
	/// A data type to represent a single
	/// RSS item in a RSS feed. See <see cref="RssReader">RssReader</see> for
	/// properties of a RSS item which have not yet been implemented 
	/// in this version of the the RssReader class. The descriptions for
	/// the properties of RssItem are para-phrased from the 
	/// <see href="http://blogs.law.harvard.edu/tech/rss">RSS 2 specification.</see>
	/// </summary>
	/// <remarks>
	/// The following elements of a RSS item aren't
	/// supported by this version of RssReader:
	/// <list type="bullet">
	/// <item>category (can have domain attribute)</item>
	/// <item>enclosure ( has attributes: url,length,type )</item>
	/// <item>source (has attributes: url)</item>
	/// </list>
	/// </remarks>
	[Serializable()]
	public struct RssItem
	{
		/// <summary>
		/// The title of the item.
		/// </summary>
		public string Title;
		/// <summary>
		/// The item synopsis.
		/// </summary>
		public string Description;
		/// <summary>
		/// The URL of the item.
		/// </summary>
		public string Link;
		/// <summary>
		/// Email address of the author of the item. 
		/// </summary>
		public string Author;
		/// <summary>
		/// URL of a page for comments relating to the item
		/// </summary>
		public string Comments;
		/// <summary>
		/// Indicates when the item was published. 
		/// </summary>
		public string Pubdate;
		/// <summary>
		/// A string that uniquely identifies the item.
		/// </summary>
		public string Guid;
	}

	/// <summary>
	/// Represents a collection of RSS items for
	/// the RSS feed.
	/// </summary>
	[Serializable()]
	public class RssItems : CollectionBase
	{
		public RssItem this[int item]
		{
			get
			{
				return this.getItem(item);
			}
		}

		public void Add(RssItem rssItem)
		{
			List.Add(rssItem);
		}

		public bool Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{	
				return false;
			}
			else
			{
				List.RemoveAt(index);
				return true;
			}
		}

		private RssItem getItem(int Index)
		{
			return (RssItem) List[Index];
		}

	}
	#endregion
}
