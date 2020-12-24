using System;

namespace RxFair.Utility.Extension
{
  public static class StringExtension
  {
    /// <summary>
    ///  Convert to string if string is null return empty string
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static string NullToString(this string val)
    {
      return (val + "").Trim();
    }

    /// <summary>
    /// Covert given string to Nullable Short
    /// </summary>
    /// <param name="val">String value</param>
    /// <returns></returns>
    public static short? ToShortCustom(this string val)
    {
      if (string.IsNullOrEmpty(val)) return null;
      try
      {
        var objShort = Convert.ToInt16(val);
        return objShort;
      }
      catch (Exception ex)
      {
          ex.Message.ToString();
        return null;
      }
    }

    /// <summary>
    /// Convert given string to Nullable int
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static int? ToInt32Custom(this string val)
    {
      if (string.IsNullOrEmpty(val)) return null;
      try
      {
        var objShort = Convert.ToInt32(val);
        return objShort;
      }
      catch (Exception)
      {
        return null;
      }
    }

    /// <summary>
    /// Convert given string to Not Nullable Int
    /// </summary>
    /// <param name="val">String value to convert</param>
    /// <param name="nullableValue">Value when given string is null or empty by Default 0</param>
    /// <param name="defaultValue">Value when given string fails to convert in int by Default 0 </param>
    /// <returns>Return Int value</returns>
    public static int ToNonNullableInt32Custom(this string val,int nullableValue=0,int defaultValue=0)
    {
      if (string.IsNullOrEmpty(val)) return nullableValue;
      try
      {
        var objShort = Convert.ToInt32(val);
        return objShort;
      }
      catch (Exception)
      {
        return defaultValue;
      }
    }
    
    /// <summary>
    /// Convert given string True and False value matcher to Boolean value
    /// </summary>
    /// <param name="val">Value to match</param>
    /// <param name="model"></param>
    /// <param name="valueOnNull">Value to return If given string is null, Default value is false</param>
    /// <param name="defaultValue">Value to return If given string does not match any of true or false value , Default is false</param>
    /// <returns></returns>
    public static bool ToBoolFromCustomString(this string val, BooleanStringModel model,bool valueOnNull=false,bool defaultValue=false)
    {
      if (string.IsNullOrEmpty(val)) return valueOnNull;
      
        if (val == model.TrueTypeString)
          return true;
        return val != model.FalseTypeString && defaultValue;
    }

    public static bool? ToNullableBoolFromCustomString(this string val, BooleanStringModel model, bool valueOnNull = false, bool? defaultValue = null)
    {
      if (string.IsNullOrEmpty(val)) return null;
      if (val == model.TrueTypeString)
        return true;
      return val == model.FalseTypeString ? false : defaultValue;
    }

  }

  public class BooleanStringModel
  {
    public string TrueTypeString { get; set; }
    public string FalseTypeString { get; set; }

    public BooleanStringModel(string trueTypeString, string falseTypeString)
    {
      TrueTypeString = trueTypeString;
      FalseTypeString = falseTypeString;
    }
  }

}
