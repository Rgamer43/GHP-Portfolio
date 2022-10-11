using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Rigidbody2DExt {

    public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius) {
        ForceMode2D mode = ForceMode2D.Force;
        var explosionDir = rb.position - explosionPosition;
        var explosionDistance = explosionDir.magnitude;

        explosionDir /= explosionDistance;

        rb.AddForce(Mathf.Lerp(0, explosionForce, 1) * explosionDir, mode);
    }
}
