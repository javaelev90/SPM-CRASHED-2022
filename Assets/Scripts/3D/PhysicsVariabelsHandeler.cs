using UnityEngine;
using UnityEditor;
using System.IO;

public class PhysicsVariabelsHandeler
{

    public static void SavePhysics(string fileName, float skinWidth, float groundCheckDistance, float mass, float gravity, float airResistance, float decelerationFactor,
        float accelerationFactor, float staticFrictionCoefficient, float kineticFrictionCoefficient)
    {
        if (!File.Exists("Assets/Scripts/3D/PhysicsVariabels/" + fileName + ".txt"))
        {
            string path = "Assets/Scripts/3D/PhysicsVariabels/" + fileName + ".txt";
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine("Properties of body");
            writer.WriteLine("skinWidth: " + skinWidth);
            writer.WriteLine("groundCheckDistance: " + groundCheckDistance);

            writer.WriteLine("\nPhysics");
            writer.WriteLine("mass: " + mass);
            writer.WriteLine("gravity: " + gravity);
            writer.WriteLine("airResistance: " + airResistance);
            writer.WriteLine("decelerationFactor: " + decelerationFactor);
            writer.WriteLine("accelerationFactor: " + accelerationFactor);
            writer.WriteLine("staticFrictionCoefficient: " + staticFrictionCoefficient);
            writer.WriteLine("kineticFrictionCoefficient: " + kineticFrictionCoefficient);

            writer.WriteLine("\nNotes:\n...");
            writer.Close();
        }
        else
        {
            Debug.Log("Assets/Scripts/3D/PhysicsVariabels/" + fileName + ".txt already exist");
        }

        
        
    }
    public static void ReadString(string fileName)
    {
        string path = "Assets/Scripts/3D/PhysicsVariabels/" + fileName + ".txt";
        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
}
