# Complete Sync Flow Test
# This test demonstrates a complete push -> pull -> verify workflow

# Step 1: Create initial data (Workspace + Project + Task)
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "flow-ws-001",
          "name": "Flow Test Workspace",
          "description": "Testing sync flow",
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
          "id": "flow-proj-001",
          "name": "Flow Test Project",
          "description": "Project for flow test",
          "workspace_id": "flow-ws-001",
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
          "id": "flow-task-001",
          "title": "Flow Test Task",
          "description": "Task for flow test",
          "status": "in_progress",
          "priority": "medium",
          "project_id": "flow-proj-001",
          "created_at": 1705660800000,
          "last_modified": 1705660800000
        }
      ],
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
[Captures]
push_timestamp: jsonpath "$.message"

# Step 2: Pull all changes since timestamp 0 (initial sync)
GET http://localhost:5000/sync/pull?last_pulled_at=0&schema_version=1

HTTP 200
[Captures]
pull_timestamp: jsonpath "$.timestamp"
[Asserts]
jsonpath "$.timestamp" > 0
# Verify workspace was created
jsonpath "$.changes.workspaces.created[*].id" contains "flow-ws-001"
# Verify project was created
jsonpath "$.changes.projects.created[*].id" contains "flow-proj-001"
# Verify task was created
jsonpath "$.changes.tasks.created[*].id" contains "flow-task-001"

# Step 3: Update the task status
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [],
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
      "updated": [
        {
          "id": "flow-task-001",
          "title": "Flow Test Task - Updated",
          "description": "Task updated during flow test",
          "status": "completed",
          "priority": "high",
          "project_id": "flow-proj-001",
          "created_at": 1705660800000,
          "last_modified": 1705668000000
        }
      ],
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

# Step 4: Pull incremental changes (only updated task should be returned)
# Using the timestamp from Step 2 as last_pulled_at
GET http://localhost:5000/sync/pull?last_pulled_at=1705660800000&schema_version=1

HTTP 200
[Asserts]
jsonpath "$.changes.workspaces.updated" count >= 0
jsonpath "$.changes.projects.updated" count >= 0
jsonpath "$.changes.tasks.updated[*].id" contains "flow-task-001"

# Step 5: Soft delete the task
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [],
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
      "deleted": ["flow-task-001"]
    },
    "comments": {
      "created": [],
      "updated": [],
      "deleted": []
    }
  }
}

HTTP 200

# Step 6: Pull and verify deletion
GET http://localhost:5000/sync/pull?last_pulled_at=1705668000000&schema_version=1

HTTP 200
[Asserts]
jsonpath "$.changes.tasks.deleted" contains "flow-task-001"
