using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawParameterLine : MonoBehaviour {
    public GameObject curveLinePrefab;
    public GameObject linePointPrefab;

    public static DrawParameterLine Instance;


    void Awake() {
        Instance = this;
        connections_ = new List<ParameterConnection>();
    }

    public ParameterConnection CreateParameterConnectionLine(Transform source, Transform destination) {
        var connection = ParameterConnection.GetConnection(connections_, source);
        if (connection != null && connection.destination != destination) {
            DestroyParameterConnectionLine(connection);
            connection = null;
        }
        if(connection == null) { 
            var newParamConnection = new ParameterConnection(createLine(), source, destination);
            newParamConnection.line.transform.position = source.position;
            createLinePoint(newParamConnection.line.transform, source);
            createLinePoint(newParamConnection.line.transform, destination);
            connections_.Add(newParamConnection);
            connection = newParamConnection;
        }
        return connection;    
    }

    void DestroyParameterConnectionLine(ParameterConnection connection) {
        if (connection != null && connections_.Contains(connection)) {
            connections_.Remove(connection);
            Destroy(connection.line);
        }
    }

    public void DestroyParameterConnectionLine(Transform source) {
        for (int i = 0; i < connections_.Count; i++) {
            if (connections_[i].source == source) {
                Destroy(connections_[i].line);
                connections_.Remove(connections_[i]);                
                break;
            }
        }
    }


    private GameObject createLine() {
        return Instantiate(curveLinePrefab);
    }

    private GameObject createLinePoint(Transform lineTrans, Transform source = null) {
        var linePoint = Instantiate(linePointPrefab);
        linePoint.transform.parent = lineTrans;
        if (source != null) {
            createWirePointFollower(linePoint, source);
        }
        return linePoint;
    }

    private void createWirePointFollower(GameObject linePoint, Transform source) {
        //var pointFollower = linePoint.AddComponent<WirePointFollower>();
        //pointFollower.source = source;
        //pointFollower.wirePoint = pointFollower.transform;
    }

    List<ParameterConnection> connections_;

    public class ParameterConnection {
        public GameObject line;
        public Transform source;
        public Transform destination;

        public ParameterConnection(GameObject line_, Transform source_, Transform dest_) {
            line = line_;
            source = source_;
            destination = dest_;
        }

        public static ParameterConnection GetConnection(List<ParameterConnection> connections, Transform source) {
            for (int i = 0; i < connections.Count; i++) {
                if (connections[i].source == source) {
                    return connections[i];
                }
            }
            return null;
        }
    }
}
