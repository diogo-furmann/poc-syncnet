# Test: Push - Create Workspace
# This test creates a new workspace

POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "550e8400-e29b-41d4-a716-446655440001",
          "name": "Test Workspace",
          "description": "A test workspace",
          "created_at": 1705660800000,
          "last_modified": 1705660800000
        }
      ],
      "updated": [],
      "deleted": []
    },
    "projects": {
      "created": [],
      "updated": [],
      "deleted": []
    },
    "tasks": {
      "created": [],
      "updated": [],
      "deleted": []
    },
    "comments": {
      "created": [],
      "updated": [],
      "deleted": []
    }
  }
}

HTTP 200
[Asserts]
jsonpath "$.message" == "Changes synchronized successfully"

# Test: Push - Create Project with Workspace
# This test creates a project linked to a workspace

POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "550e8400-e29b-41d4-a716-446655440002",
          "name": "Main Workspace",
          "description": "Main workspace for testing",
          "created_at": 1705660800000,
          "last_modified": 1705660800000
        }
      ],
      "updated": [],
      "deleted": []
    },
    "projects": {
      "created": [
        {
          "id": "660f9511-f39c-52e5-b827-557766551112",
          "name": "Test Project",
          "description": "A test project",
          "workspace_id": "550e8400-e29b-41d4-a716-446655440002",
          "created_at": 1705660800000,
          "last_modified": 1705660800000
        }
      ],
      "updated": [],
      "deleted": []
    },
    "tasks": {
      "created": [],
      "updated": [],
      "deleted": []
    },
    "comments": {
      "created": [],
      "updated": [],
      "deleted": []
    }
  }
}

HTTP 200

# Test: Push - Update Workspace
# This test updates an existing workspace

POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [],
      "updated": [
        {
          "id": "550e8400-e29b-41d4-a716-446655440001",
          "name": "Updated Workspace Name",
          "description": "Updated description",
          "created_at": 1705660800000,
          "last_modified": 1705664400000
        }
      ],
      "deleted": []
    },
    "projects": {
      "created": [],
      "updated": [],
      "deleted": []
    },
    "tasks": {
      "created": [],
      "updated": [],
      "deleted": []
    },
    "comments": {
      "created": [],
      "updated": [],
      "deleted": []
    }
  }
}

HTTP 200

# Test: Push - Create Complete Hierarchy
# This test creates a complete hierarchy: Workspace > Project > Task > Comment

POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "ws-hierarchy-001",
          "name": "Hierarchy Test Workspace",
          "description": "Testing full hierarchy",
          "created_at": 1705660800000,
          "last_modified": 1705660800000
        }
      ],
      "updated": [],
      "deleted": []
    },
    "projects": {
      "created": [
        {
          "id": "proj-hierarchy-001",
          "name": "Hierarchy Test Project",
          "description": "Project for hierarchy test",
          "workspace_id": "ws-hierarchy-001",
          "created_at": 1705660800000,
          "last_modified": 1705660800000
        }
      ],
      "updated": [],
      "deleted": []
    },
    "tasks": {
      "created": [
        {
          "id": "task-hierarchy-001",
          "title": "Test Task",
          "description": "Task for testing",
          "status": "pending",
          "priority": "high",
          "project_id": "proj-hierarchy-001",
          "created_at": 1705660800000,
          "last_modified": 1705660800000
        }
      ],
      "updated": [],
      "deleted": []
    },
    "comments": {
      "created": [
        {
          "id": "comment-hierarchy-001",
          "content": "This is a test comment",
          "task_id": "task-hierarchy-001",
          "created_at": 1705660800000,
          "last_modified": 1705660800000
        }
      ],
      "updated": [],
      "deleted": []
    }
  }
}

HTTP 200

# Test: Push - Soft Delete (Workspace)
# This test soft-deletes a workspace

POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [],
      "updated": [],
      "deleted": ["550e8400-e29b-41d4-a716-446655440001"]
    },
    "projects": {
      "created": [],
      "updated": [],
      "deleted": []
    },
    "tasks": {
      "created": [],
      "updated": [],
      "deleted": []
    },
    "comments": {
      "created": [],
      "updated": [],
      "deleted": []
    }
  }
}

HTTP 200

# Test: Push - Invalid Request (Null Changes)
# This test verifies error handling for invalid requests

POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": null
}

HTTP 400
[Asserts]
jsonpath "$.status" == 400
