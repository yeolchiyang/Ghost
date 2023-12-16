using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserBuilder
{
    private string name;
    private string email;
    private int age;

    public UserBuilder setName(string name)
    {
        this.name = name;
        return this;
    }

    public UserBuilder setEmail(string email)
    {
        this.email = email;
        return this;
    }

    public UserBuilder setAge(int age)
    {
        this.age = age;
        return this;
    }

    public User build()
    {
        return new User(name, email, age);
    }
}
