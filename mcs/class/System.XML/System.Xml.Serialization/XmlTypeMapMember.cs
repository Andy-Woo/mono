//
// XmlTypeMapMember.cs: 
//
// Author:
//   Lluis Sanchez Gual (lluis@ximian.com)
//
// (C) 2002, 2003 Ximian, Inc.  http://www.ximian.com
//

using System;
using System.Collections;
using System.Reflection;

namespace System.Xml.Serialization
{
	// XmlTypeMapMember
	// A member of a class that must be serialized

	internal class XmlTypeMapMember
	{
		string _name;
		int _index;
		TypeData _typeData;
		MemberInfo _member;
		MemberInfo _specifiedMember;
		object _defaultValue = System.DBNull.Value;
		string documentation;
		bool _isOptional;
		bool _isReturnValue;

		public XmlTypeMapMember()
		{
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		
		public object DefaultValue
		{
			get { return _defaultValue; }
			set { _defaultValue = value; }
		}

		public string Documentation
		{
			set { documentation = value; }
			get { return documentation; }
		}

		public bool IsReadOnly (Type type)
		{
			if (_member == null) InitMember (type);
			return (_member is PropertyInfo) && !((PropertyInfo)_member).CanWrite;
		}

		public static object GetValue (object ob, string name)
		{
			MemberInfo[] mems = ob.GetType().GetMember (name, BindingFlags.Instance|BindingFlags.Public);
			if (mems[0] is PropertyInfo) return ((PropertyInfo)mems[0]).GetValue (ob, null);
			else return ((FieldInfo)mems[0]).GetValue (ob);
		}

		public object GetValue (object ob)
		{
			if (_member == null) InitMember (ob.GetType());
			if (_member is PropertyInfo) return ((PropertyInfo)_member).GetValue (ob, null);
			else return ((FieldInfo)_member).GetValue (ob);
		}

		public void SetValue (object ob, object value)
		{
			if (_member == null) InitMember (ob.GetType());
			if (_member is PropertyInfo) ((PropertyInfo)_member).SetValue (ob, value, null);
			else ((FieldInfo)_member).SetValue (ob, value);
		}

		void InitMember (Type type)
		{
			MemberInfo[] mems = type.GetMember (_name, BindingFlags.Instance|BindingFlags.Public);
			_member = mems[0];
			
			mems = type.GetMember (_name + "Specified", BindingFlags.Instance|BindingFlags.Public);
			if (mems.Length > 0) _specifiedMember = mems[0];
		}

		public TypeData TypeData
		{
			get { return _typeData; }
			set { _typeData = value; }
		}

		public int Index
		{
			get { return _index; }
			set { _index = value; }
		}
		
		public bool IsOptionalValueType
		{
			get { return _isOptional; }
			set { _isOptional = value; }
		}
		
		public bool IsReturnValue
		{
			get { return _isReturnValue; }
			set { _isReturnValue = value; }
		}
		
		public void CheckOptionalValueType (Type type)
		{
			if (_member == null) InitMember (type);
			_isOptional = (_specifiedMember != null);
		}
		
		public bool GetValueSpecified (object ob)
		{
			if (_specifiedMember is PropertyInfo) return (bool) ((PropertyInfo)_specifiedMember).GetValue (ob, null);
			else return (bool) ((FieldInfo)_specifiedMember).GetValue (ob);
		}

		public void SetValueSpecified (object ob, bool value)
		{
			if (_specifiedMember is PropertyInfo) ((PropertyInfo)_specifiedMember).SetValue (ob, value, null);
			else ((FieldInfo)_specifiedMember).SetValue (ob, value);
		}
	}
}
