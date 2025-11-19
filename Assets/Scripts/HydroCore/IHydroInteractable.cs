using UnityEngine;

public interface IHydroInteractable
{
    /*
     * This is the main method for interactions.
     * The HydroCore will call this method on the object it hits.
     *
     * @param state - What kind of water is hitting the object? (Jet, Ice, etc.)
     * @param hitPoint - Where in the world is the impact?
     * @param hitNormal - What is the angle of the surface we hit?
     */
    void OnHydroHit(HydroCore playerCore, WaterState state, Vector3 hitPoint, Vector3 hitNormal);

    /*
     * We add a second method for when the player *stops* hitting 
     * the object. This is useful for a continuous jet
     * (e.g., the turbine stops spinning when the jet stops).
     */
    void OnHydroHitStop(WaterState state);
}