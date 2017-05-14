using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace STB.Basics
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: EditorBasicFunctions
    /// # Compilation on functions used by editor
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class BasicFunctions : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetIsMainCharacter
        /// # 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool GetIsMainCharacter(string name)
        {
            return ((name == "MainCharacter") || (name == "CharacterRigidBody"));
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetIgnoreRaycastLayerForAllChildBoxCollidersInside
        /// # 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetIgnoreRaycastLayerForAllChildBoxCollidersInside(Transform src)
        {
            if (src.gameObject && src.GetComponent<BoxCollider>()) src.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            foreach (Transform child in src)
            {
                SetIgnoreRaycastLayerForAllChildBoxCollidersInside(child);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetIgnoreRaycastLayer
        /// # 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetIgnoreRaycastLayer(GameObject go)
        {
            go.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetVelocityToAllChildsInRadiusOfPosition
        /// # Set velocity to ragdoll's rigidbodys
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetVelocityToAllChildsInRadiusOfPosition(Transform src, Vector3 o, Vector3 p, float speed, float r, bool basedOnRadious)
        {
            //Debug.Log("speed: " + speed);

            bool detected = false;

            if (src.GetComponent<Rigidbody>() && !src.GetComponent<Rigidbody>().isKinematic)
            {
                if (!basedOnRadious || (src.transform.position - p).magnitude < r)
                {
                    Vector3 vel = (src.transform.position - o);
                    vel = speed * vel.normalized;

                    //Debug.Log("velocity para "+src.name + " con velocity : "+vel);

                    src.GetComponent<Rigidbody>().velocity = vel;
                    //Debug.Log("change v in "+src.name);

                    detected = true;
                }
            }

            if (!detected)
            {
                foreach (Transform child in src)
                {
                    SetVelocityToAllChildsInRadiusOfPosition(child, o, p, speed, r, basedOnRadious);
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CopyTransformsRecurse
        /// # Recursive copy of all needed transforms setup to preserve actual animation state when enable the
        /// # ragdoll of a character is wanted
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void CopyTransformsRecurse(Transform src, Transform dst)
        {
            dst.position = src.position;
            dst.rotation = src.rotation;

            if (dst.GetComponent<Rigidbody>())
            {
                dst.GetComponent<Rigidbody>().position = src.position;
                dst.GetComponent<Rigidbody>().rotation = src.rotation;
                //dst.rigidbody.velocity = Vector3.zero;
                //dst.rigidbody.angularVelocity = Vector3.zero;
            }

            foreach (Transform child in dst)
            {
                // Match the transform with the same name
                var curSrc = src.Find(child.name);
                if (curSrc)

                    CopyTransformsRecurse(curSrc, child);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DeactivateChildsRigidBody
        /// # Used to disable ragdoll
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void DeactivateChildsRigidBody(Transform src)
        {
            if (src.GetComponent<Rigidbody>())
            {
                src.GetComponent<Rigidbody>().isKinematic = true;
            }

            if (src.GetComponent<Collider>())
            {
                if (!src.GetComponent<Collider>().isTrigger)
                {
                    src.GetComponent<Collider>().enabled = false;
                }
            }

            foreach (Transform child in src)
            {
                DeactivateChildsRigidBody(child);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ActivateChildsRigidBody
        /// # Used to enable ragdoll
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void ActivateChildsRigidBody(Transform src)
        {
            if (src.GetComponent<Rigidbody>())
            {
                src.GetComponent<Rigidbody>().isKinematic = false;
            }

            if (src.GetComponent<Collider>())
            {
                if (!src.GetComponent<Collider>().isTrigger)
                {
                    src.GetComponent<Collider>().enabled = true;
                }
            }

            foreach (Transform child in src)
            {
                ActivateChildsRigidBody(child);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// IgnoreCollisionsWith
        /// # 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void IgnoreCollisionsWith(Transform src, Collider toIgnore, bool state)
        {
            foreach (Collider col in src.GetComponents<Collider>())
            {
                //Debug.Log(col.gameObject.name + " -> " + toIgnore.gameObject.name);

                if (col.gameObject.activeSelf && col.enabled && toIgnore.gameObject.activeSelf && toIgnore.enabled) Physics.IgnoreCollision(col, toIgnore, !state);
            }

            foreach (Transform child in src)
            {
                IgnoreCollisionsWith(child, toIgnore, state);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CopyComponent
        /// # 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            var dst = destination.GetComponent(type) as T;
            if (!dst) dst = destination.AddComponent(type) as T;
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(dst, prop.GetValue(original, null), null);
            }

            return dst as T;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetAngleBetweenTwoVector
        /// # 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static float GetAngleBetweenTwoVector(Vector3 vec1, Vector3 vec2)
        {
            Vector3 diference = vec2 - vec1;
            float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;

            return Vector3.Angle(Vector2.right, diference) * sign;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetCentroid
        /// # 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Vector3 GetCentroid(List<Transform> targets)
        {
            if (targets.Count <= 0) return Vector3.zero;

            Vector3 centroid;
            Vector3 minPoint = targets[0].position;
            Vector3 maxPoint = targets[0].position;

            for (int i = 1; i < targets.Count; i++)
            {
                Vector3 pos = targets[i].position;
                if (pos.x < minPoint.x)
                    minPoint.x = pos.x;
                if (pos.x > maxPoint.x)
                    maxPoint.x = pos.x;
                if (pos.y < minPoint.y)
                    minPoint.y = pos.y;
                if (pos.y > maxPoint.y)
                    maxPoint.y = pos.y;
                if (pos.z < minPoint.z)
                    minPoint.z = pos.z;
                if (pos.z > maxPoint.z)
                    maxPoint.z = pos.z;
            }

            centroid = minPoint + 0.5f * (maxPoint - minPoint);


            return centroid;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetParentGenericMainCharacter
        /// # return the parent GenericMainCharacter giving a transform
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Generic.GenericMainCharacter GetParentGenericMainCharacter(Transform t)
        {
            Transform actualParent = t.transform;

            while (actualParent != null)
            {
                Generic.GenericMainCharacter actualGenericMainCharacter = actualParent.gameObject.GetComponent<Generic.GenericMainCharacter>();

                if (actualGenericMainCharacter)
                {
                    return actualGenericMainCharacter;
                }

                actualParent = actualParent.parent;
            }

            return null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetParentGenericVehicleController
        /// # return the parent GenericVehicleController giving a transform
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Generic.GenericVehicleController GetParentGenericVehicleController(Transform t)
        {
            Transform actualParent = t.transform;

            while (actualParent != null)
            {
                Generic.GenericVehicleController actualGenericVehicleController = actualParent.gameObject.GetComponent<Generic.GenericVehicleController>();

                if (actualGenericVehicleController)
                {
                    return actualGenericVehicleController;
                }

                actualParent = actualParent.parent;
            }

            return null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetTransformInChildsByName
        /// # Return a child transform inside another transform using a name
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Transform GetTransformInChildsByName(Transform mainTransform, string name)
        {
            if (mainTransform.name == name)
            {
                return mainTransform;
            }

            foreach (Transform child in mainTransform)
            {
                Transform t = GetTransformInChildsByName(child, name);

                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetGameObjectInChildsByName
        /// # Return a child gameObject inside a transform using a name
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static GameObject GetGameObjectInChildsByName(Transform mainTransform, string name)
        {
            if (mainTransform.name == name)
            {
                return mainTransform.gameObject;
            }

            foreach (Transform child in mainTransform)
            {
                Transform t = GetTransformInChildsByName(child, name);

                if (t != null)
                {
                    return t.gameObject;
                }
            }

            return null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AddExtraVelocityToAllChilds
        /// # Add extra velocity to ragdoll's rigidbodys
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void AddExtraVelocityToAllChilds(Transform src, Vector3 velocity)
        {
            if (src.GetComponent<Rigidbody>() && !src.GetComponent<Rigidbody>().isKinematic)
            {
                //Debug.Log("velocity for " + src.name + " with velocity : " + velocity);

                src.GetComponent<Rigidbody>().velocity += velocity;
            }

            foreach (Transform child in src)
            {
                AddExtraVelocityToAllChilds(child, velocity);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetAllChildRenderers
        /// # 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void GetAllChildRenderers(Transform src, ref List<Renderer> rendererList)
        {
            if (src.GetComponent<Renderer>())
            {
                rendererList.Add(src.GetComponent<Renderer>());
            }

            foreach (Transform child in src)
            {
                GetAllChildRenderers(child, ref rendererList);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DisableReceiveShadowsForAllInside
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void DisableReceiveShadowsForAllInside(Transform t)
        {
            if (t.GetComponent<Renderer>())
            {
                t.GetComponent<Renderer>().receiveShadows = false;
            }

            foreach (Transform child in t)
            {
                DisableReceiveShadowsForAllInside(child);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DisableCastShadowsForAllInside
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void DisableCastShadowsForAllInside(Transform t)
        {
            if (t.GetComponent<Renderer>())
            {
                t.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            foreach (Transform child in t)
            {
                DisableCastShadowsForAllInside(child);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetStringNormalizedCounterForMiles
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetStringNormalizedCounterForMiles(int counter)
        {
            if (counter < 10) return ("000" + counter.ToString());
            if (counter < 100) return ("00" + counter.ToString());
            if (counter < 1000) return ("0" + counter.ToString());

            return counter.ToString();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// FindChildWithName
        /// # Return a gameobject child with the given name
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static GameObject FindChildWithName(GameObject go, string name)
        {
            foreach (Transform childTransform in go.transform)
            {
                GameObject child = childTransform.gameObject;

                if (child.name == name)
                    return child;
            }

            return null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetFirstMeshFilterInChilds
        /// # Return a mesh filter inside a transform using a name
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static MeshFilter GetFirstMeshFilterInChilds(Transform mainTransform)
        {
            MeshFilter actualMeshFilter = mainTransform.GetComponent<MeshFilter>();

            if (actualMeshFilter != null)
            {
                return actualMeshFilter;
            }

            foreach (Transform child in mainTransform)
            {
                MeshFilter t = GetFirstMeshFilterInChilds(child);

                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetMainMaterial
        /// # Return first material found for this gameobject
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Material GetMainMaterial(Transform mainTransform)
        {
            MeshRenderer actualMeshRenderer = mainTransform.GetComponent<MeshRenderer>();

            if (actualMeshRenderer != null)
            {
                return actualMeshRenderer.sharedMaterial;
            }

            foreach (Transform child in mainTransform)
            {
                Material t = GetMainMaterial(child);

                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetSnapNearObject
        /// # Return the nearest object of another object
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static GameObject GetSnapNearObject(GameObject go, float radius)
        {
            Collider[] hitColliders = Physics.OverlapSphere(go.transform.position, radius);

            GameObject nearObject = null;
            float minDistance = 99999;

            foreach (Collider col in hitColliders)
            {
                if (col.gameObject != go.gameObject)
                {
                    //Debug.Log (col.name);

                    float actualDistance = Vector3.Distance(go.transform.position, col.gameObject.transform.position);

                    if (actualDistance < minDistance)
                    {
                        minDistance = actualDistance;
                        nearObject = col.gameObject;
                    }
                }
            }


            return nearObject;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetNearVertexPosition
        /// # Return near object's vertex position from another object
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Vector3 GetNearVertexPosition(GameObject object1, GameObject object2)
        {
            Vector3 nearVertexPosition = BasicDefines.TOO_FAR_POSITION;

            MeshFilter mf = object1.GetComponent<MeshFilter>();

            if (mf && mf.sharedMesh)
            {
                float minVertexDistance = 99999;

                foreach (Vector3 vertice in mf.mesh.vertices)
                {
                    Vector3 verticeWorldPosition = object2.transform.position + Vector3.Scale(vertice, object2.transform.localScale);

                    float actualVertexDistance = Vector3.Distance(object1.transform.position, verticeWorldPosition);

                    if (actualVertexDistance < minVertexDistance)
                    {
                        minVertexDistance = actualVertexDistance;
                        nearVertexPosition = verticeWorldPosition;
                    }
                }
            }


            return nearVertexPosition;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetNearFacePoint
        /// # Return near object's face position from another object
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Vector3 GetNearFacePoint(GameObject object1, GameObject object2)
        {
            Vector3 nearFacePosition = BasicDefines.TOO_FAR_POSITION;

            MeshFilter mf = object1.GetComponent<MeshFilter>();

            if (mf && mf.sharedMesh)
            {
                float minFaceDistance = 99999;

                for (int i = 0; i < mf.sharedMesh.triangles.Count(); i += 3)
                {
                    Vector3 trisA = mf.sharedMesh.vertices[mf.sharedMesh.triangles[i]];
                    Vector3 trisB = mf.sharedMesh.vertices[mf.sharedMesh.triangles[i + 1]];
                    Vector3 trisC = mf.sharedMesh.vertices[mf.sharedMesh.triangles[i + 2]];

                    Vector3 faceLocalPosition = (trisA + trisB + trisC) / 3;

                    Vector3 faceWorldPosition = object2.transform.position + faceLocalPosition;

                    float actualFaceDistance = Vector3.Distance(object1.transform.position, faceWorldPosition);

                    if (actualFaceDistance < minFaceDistance)
                    {
                        minFaceDistance = actualFaceDistance;
                        nearFacePosition = faceWorldPosition;
                    }
                }
            }


            return nearFacePosition;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetFormatedDateTime
        /// # Return formated actual date time
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetFormatedDateTime()
        {
            string dateTime = System.DateTime.Now.ToString();

            string fomartedDateTime = "";

            for (int i = 0; i < dateTime.Length; i++)
            {
                if ((dateTime[i] == ' ') || (dateTime[i] == '\\') || (dateTime[i] == '/') || (dateTime[i] == ':'))
                {
                    fomartedDateTime += '_';
                }
                else
                {
                    fomartedDateTime += dateTime[i];
                }
            }


            return fomartedDateTime;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SphericalToCartesian
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Vector3 SphericalToCartesian(float radius, float polar, float elevation)
        {
            Vector3 outCart = Vector3.zero;

            float a = radius * Mathf.Cos(elevation);
            outCart.x = a * Mathf.Cos(polar);
            outCart.y = radius * Mathf.Sin(elevation);
            outCart.z = a * Mathf.Sin(polar);


            return outCart;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CreateContainerIfNotExists
        /// # Creates a new empty gameobject to contain others
        /// # Return created or founded container
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static GameObject CreateContainerIfNotExists(string name)
        {
            GameObject container = GameObject.Find(name);

            if (!container)
            {
                container = new GameObject(name);
            }

            return container;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DestroyAllChildsCollidersThatNotAreTriggers
        /// # Used to destroy all colliders that not are triggers in a transform (including childrens)
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void DestroyAllChildsCollidersThatNotAreTriggers(Transform src)
        {
            if (src.GetComponent<Collider>() && !src.GetComponent<Collider>().isTrigger)
            {
                GameObject.DestroyImmediate(src.GetComponent<Collider>());
            }

            foreach (Transform child in src)
            {
                DestroyAllChildsCollidersThatNotAreTriggers(child);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// RenderVolume
        /// # render volume shape
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void RenderVolume(ref Transform shape, Vector3 p1, Vector3 p2, float radius, Vector3 dir, float distance)
        {
            if (!shape)
            {
                // if shape doesn't exist yet, create it
                shape = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                GameObject.Destroy(shape.GetComponent<Collider>()); // no collider, please!
                                                                    //shape.GetComponent<Renderer>().material = mat; // assign the selected material to it
            }

            Vector3 scale; // calculate desired scale
            float diam = 2 * radius; // calculate capsule diameter
            scale.x = diam; // width = capsule diameter
            scale.y = Vector3.Distance(p2, p1) + diam; // capsule height
            scale.z = distance + diam; // volume length
            shape.localScale = scale; // set the rectangular volume size

            // set volume position and rotation
            shape.position = (p1 + p2 + dir.normalized * distance) / 2;
            shape.rotation = Quaternion.LookRotation(dir, p2 - p1);
            shape.GetComponent<Renderer>().enabled = true; // show it
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetParentWheelCollider
        /// # return the parent WheelCollider giving a transform
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static WheelCollider GetParentWheelCollider(Transform t)
        {
            Transform actualParent = t.transform;

            while (actualParent != null)
            {
                WheelCollider actualWheelCollider = actualParent.gameObject.GetComponent<WheelCollider>();

                if (actualWheelCollider)
                {
                    return actualWheelCollider;
                }

                actualParent = actualParent.parent;
            }

            return null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetAspectRatio
        /// # Return actual screen aspect ratio
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static float GetAspectRatio()
        {
            return ((float)Screen.width / (float)Screen.height);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// StopSound
        /// # Stop sound
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void StopSound(AudioSource source)
        {
            source.Stop();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// PlaySound
        /// # Play sound
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void PlaySound(AudioSource source, AudioClip sound, float volume)
        {
            source.volume = volume;
            source.clip = sound;

            if (source.isActiveAndEnabled) source.Play();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DeleteAllChilds
        /// # Delete all childs inside a transform
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void DeleteAllChilds(Transform mainTransform)
        {
            //Debug.Log(mainTransform.childCount

            foreach (Transform child in mainTransform)
            {
                //Debug.Log ("Destroy child: " + child.gameObject.name);
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetMeshFilterChildListInATransform
        /// # Return all mesh filter inside a transform
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void GetMeshFilterChildListInATransform(Transform t, ref List<MeshFilter> meshFilterList)
        {
            if (t.gameObject.GetComponent<MeshFilter>())
            {
                meshFilterList.Add(t.gameObject.GetComponent<MeshFilter>());
            }

            foreach (Transform tChild in t)
            {
                GetMeshFilterChildListInATransform(tChild, ref meshFilterList);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// StopChildsRigidBody
        /// # Set to zero all velocitys of a rigidbody
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void StopChildsRigidBody(Transform src)
        {
            if (src.GetComponent<Rigidbody>())
            {
                src.GetComponent<Rigidbody>().angularDrag = 0.0f;
                src.GetComponent<Rigidbody>().velocity = Vector3.zero;
                src.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }

            foreach (Transform child in src)
            {
                StopChildsRigidBody(child);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetVelocityToAllChildsInRadiusOfScale
        /// # Set velocity to ragdoll's rigidbodys
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetVelocityToAllChildsInRadiusOfScale(Transform src, Vector3 o, Vector3 p, float speed, float r)
        {
            bool detected = false;

            if (src.GetComponent<Rigidbody>() && !src.GetComponent<Rigidbody>().isKinematic)
            {
                if ((src.transform.position - p).magnitude < r)
                {
                    Vector3 vel = (p - o);
                    vel = speed * vel.normalized;

                    //print("velocity para "+src.name + " con velocity : "+vel);

                    src.GetComponent<Rigidbody>().velocity = vel;
                    //print("change v in "+src.name);

                    detected = true;
                }
            }

            if (!detected)
            {
                foreach (Transform child in src)
                {
                    SetVelocityToAllChildsInRadiusOfScale(child, o, p, speed, r);
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetLayerToAllChilds
        /// # Set a layer to all the childs giving a transform parent
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetLayerToAllChilds(Transform src, string layerName, string warningText)
        {
#if UNITY_EDITOR
            int layerIndex = LayerMask.NameToLayer(layerName);

            if (layerIndex == -1)
            {
                if (warningText.Length > 0) Debug.Log(warningText);
            }
            else
            {
                if (!src.gameObject.GetComponent<Light>()) src.gameObject.layer = layerIndex;
            }
#endif

            foreach (Transform child in src)
            {
                SetLayerToAllChilds(child, layerName, "");
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// IgnoreCollisions
        /// # Ignore collisions between two colliders
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void IgnoreCollisions(Collider collA, Collider collB)
        {
            bool ignore = true;

            if (!collA)
            {
                ignore = false;
            }

            if (!collB)
            {
                ignore = false;
            }

            if (collA && !collA.enabled)
            {
                ignore = false;
            }

            if (collB && !collB.enabled)
            {
                ignore = false;
            }

            if (ignore)
            {
                //Debug.Log ("IgnoreCollisions between " + collA.name + " and " + collB.name);
                Physics.IgnoreCollision(collA, collB);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetVelocityToAllChildsInRadiusOfPosition
        /// # Set velocity to ragdoll's rigidbodys
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetVelocityToAllChildsInRadiusOfPosition(Transform src, Vector3 o, Vector3 p, float speed, float r)
        {
            bool detected = false;

            if (src.GetComponent<Rigidbody>() && !src.GetComponent<Rigidbody>().isKinematic)
            {
                if ((src.transform.position - p).magnitude < r)
                {
                    Vector3 vel = (p - o);
                    vel = speed * vel.normalized;

                    //Debug.Log("velocity for "+src.name + " with velocity : "+vel);

                    src.GetComponent<Rigidbody>().velocity = vel;
                    //Debug.Log("change v in "+src.name);

                    detected = true;
                }
            }

            if (!detected)
            {
                foreach (Transform child in src)
                {
                    SetVelocityToAllChildsInRadiusOfPosition(child, o, p, speed, r);
                }
            }
        }
    }
}
