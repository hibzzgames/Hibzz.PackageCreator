using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

internal static class ReflectionTools
{
    // Standard flags
    static readonly BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    // get the instance of the object
    public static object Instance(this object obj)
    {
        // this way we can use a common interface for both static objects as well as instanced objects
        // null represents a call from the static side of things
        return (obj is Type) ? null : obj;
    }

    // get the type of the object (unless it's already a type, then just return it)
    public static Type Type(this object obj)
    {
        return (obj as Type) ?? obj.GetType();
    }

    // create a new object with the given arguments in it's constructor
    public static object New(Type typeToCreate, params object[] args)
    {
        // get the type of the args in the same order
        var argsTypes = args.Select(arg => arg.GetType()).ToArray();

        // get a matching constructor
        var constructor = typeToCreate.GetConstructor(argsTypes);
        if(constructor is null) { return null; }

        // invoke the constructor and return the result
        return constructor.Invoke(args);
    }

    // Make a call to a method with a given name on the given object with the passed in arguments
    // If you wish to make a call to a static function, pass the type in place of object
    public static object Call(object obj, string methodName, params object[] args)
    {
        // get the type of the args in the same order
        var argsTypes = args?.Select(arg => arg.GetType()).ToArray();

        // get all the available methods in the object with matching method name
        var methods = obj.Type().GetMethods() as IEnumerable<MethodInfo>;
        methods = methods.Where(method => method.Name == methodName);

        // get the first method whose param types match what's expected
        var method = methods.First(method => 
        {
            // for this method get a list of parameters and if there's a mismatch in the number
            // of arguments sent in, we immediately know that this is not a matching method
            var parameterTypes = method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
            if(parameterTypes.Length != argsTypes.Length) { return false; }

            // loop through both list and check if there's a parameter match
            for(int i = 0; i < parameterTypes.Length; i++)
            {
                if (!parameterTypes[i].IsAssignableFrom(argsTypes[i])) { return false; }
            }

            // passed all checks, so it's a match!
            return true;
        });
         
        // invoke this method, if one was provided and return it
        return method?.Invoke(obj.Instance(), args);
    }

    // Make a call to a generic method with the given name that has matching arguments on the given object
    // If you wish to make a call to a static function, pass the type in place of object
    public static object Call(object obj, string methodName, IEnumerable<Type> genericTypes, params object[] args)
    {
        // get first available generic method in the object with a matching name
        var methods = obj.Type().GetMethods();
        var method = methods.First(method => method.IsGenericMethodDefinition && method.Name == methodName);

        // make a generic method and invoke it
        var genericMethod = method?.MakeGenericMethod(genericTypes.ToArray());
        return genericMethod?.Invoke(obj.Instance(), args);
    }

    // Get the member with the given name
    public static object Get(object obj, string memberName)
    {
        // try getting the member as a property first
        var property = obj.Type().GetProperty(memberName, AllFlags);
        if(property is not null)
        {
            return property.GetValue(obj.Instance(), new object[0]);
        }

        // not a property, try getting the member as field
        var field = obj.Type().GetField(memberName, AllFlags);
        if(field is not null)
        {
            return field.GetValue(obj.Instance());
        }

        // member not a property or a field... only these members can have their value be "get"
        return null;
    }

    // Set the value on the member with the given name
    public static void Set(object obj, string memberName, object value)
    {
        var property = obj.Type().GetProperty(memberName, AllFlags);
        if (property is not null)
        {
            property.SetValue(obj.Instance(), value, new object[0]);
            return;
        }

        // not a property, try getting the member as field
        var field = obj.Type().GetField(memberName, AllFlags);
        if (field is not null)
        {
            field.SetValue(obj.Instance(), value);
            return;
        }

        // reached here mean's it's invalid
        Debug.LogWarning($"Field/Property with the name {memberName} not found in {obj.Type().FullName}");
    }

    // print the internal members of an object (for debug purposes)
    public static void PrintInternal(object obj)
    {
        // the aggregate output string
        string output = "";
        
        // loop around all members and concatenate them as strings
        var members = obj.Type().GetMembers(AllFlags);
        foreach(var member in members)
        {
            output += $"  {member}\n";
        }

        // print it to console
        Debug.Log($"{obj.Type().FullName}:\n<Color=lightblue>{output}</Color>");
    }
}