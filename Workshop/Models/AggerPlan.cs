using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Workshop.Models
{
    public class Style
    {
        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("fontFamily")]
        public string FontFamily { get; set; }

        [JsonPropertyName("fontSize")]
        public string FontSize { get; set; }
    }

    public class TextElement
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("htmlTag")]
        public string HtmlTag { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("style")]
        public Style Style { get; set; }
    }

    public class Licence
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("licenceQuantity")]
        public int LicenceQuantity { get; set; }

        [JsonPropertyName("price")]
        public TextElement Price { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class PackageItem
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("htmlTag")]
        public string HtmlTag { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("style")]
        public Style Style { get; set; }
    }

    public class ButtonAg
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("htmlTag")]
        public string HtmlTag { get; set; }

        [JsonPropertyName("googleTagManagerID")]
        public string GoogleTagManagerID { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("style")]
        public Style Style { get; set; }
    }

    public class Plan
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("title")]
        public TextElement Title { get; set; }

        [JsonPropertyName("description")]
        public TextElement Description
        {
            get; set;
        }
        [JsonPropertyName("packageItems")]
        public List<PackageItem> PackageItems { get; set; }

        [JsonPropertyName("licences")]
        public List<Licence> Licences { get; set; }

        [JsonPropertyName("buyButton")]
        public ButtonAg BuyButton { get; set; }

        [JsonPropertyName("requestContactButton")]
        public ButtonAg RequestContactButton { get; set; }

        [JsonPropertyName("arrayPos")]
        public int ArrayPos { get; set; }

        [JsonPropertyName("__v")]
        public int V { get; set; }

        [JsonPropertyName("hireApiType")]
        public int HireApiType { get; set; }
    }

    
}



