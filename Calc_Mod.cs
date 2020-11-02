//Programmer: Aviv Cohn
using System;

internal class Calculator
{
    public Double Add(double NumA, double NumB)
    {
        return NumA + NumB;
        
    }

    public Double Subtract(double NumA, double NumB)
    {
        return NumA - NumB;
    }

    public Double Divide(double NumA, double NumB)
    {
        return NumA / NumB; //Converting one number to double makes the entire operation function as double. Unnecessary to convert both numbers. Using Double rather than Decimal for uniformity.
    }

    public Double Multiply(double NumA, double NumB)
    { 
        
        return NumA * NumB; //See note above regarding Divide
    }

    public Double Square(double NumA)
    {
        return Math.Sqrt(NumA);
    }

    public double Power(ushort NumA, double NumB)
    {
        return Math.Pow(NumA, NumB);
    }
}