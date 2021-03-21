using Unity.Entities;

public struct ECS_Jobs_BoidComponent : ISharedComponentData
{
    /* An IComponentData struct should generally be very small (under 100 bytes, typically). 
   Large data, like textures and meshes, should only be stored in ISharedComponentData structs.
   https://github.com/BrianWill/LearnUnity/blob/master/ecs-jobs/ecs.md
   */

    // Add fields to your component here. Remember that:
    //
    // * A component itself is for storing data and doesn't 'do' anything.
    //
    // * To act on the data, you will need a System.
    //
    // * Data in a component must be blittable, which means a component can
    //   only contain fields which are primitive types or other blittable
    //   structs; they cannot contain references to classes.
    //
    // * You should focus on the data structure that makes the most sense
    //   for runtime use here. Authoring Components will be used for 
    //   authoring the data in the Editor.
}
