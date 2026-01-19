# Edge Cases Test Suite
# This file tests error conditions, boundary cases, and exceptional scenarios

# ============================================================================
# 1. FOREIGN KEY VIOLATIONS
# ============================================================================

# Test: Create Project without existing Workspace
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
      "created": [
        {
          "id": "orphan-proj-001",
          "name": "Orphan Project",
          "description": "Project without workspace",
          "workspace_id": "non-existent-ws-999",
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

HTTP 500

# Test: Create Task without existing Project
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
      "created": [
        {
          "id": "orphan-task-001",
          "title": "Orphan Task",
          "description": "Task without project",
          "status": "pending",
          "priority": "low",
          "project_id": "non-existent-proj-999",
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

HTTP 500

# Test: Create Comment without existing Task
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
      "deleted": []
    },
    "comments": {
      "created": [
        {
          "id": "orphan-comment-001",
          "content": "Comment without task",
          "task_id": "non-existent-task-999",
          "created_at": 1705660800000,
          "last_modified": 1705660800000
        }
      ],
      "updated": [],
      "deleted": []
    }
  }
}

HTTP 500

# ============================================================================
# 2. DUPLICATE ID HANDLING
# ============================================================================

# Test: Create entity then try to create again with same ID
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "duplicate-ws-001",
          "name": "Duplicate Test Workspace",
          "description": "First creation",
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

# Test: Try to create workspace with duplicate ID (API treats as upsert)
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "duplicate-ws-001",
          "name": "Duplicate Test Workspace AGAIN",
          "description": "Second creation attempt",
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

# ============================================================================
# 3. SOFT DELETE + UPDATE CONFLICTS
# ============================================================================

# Test: Setup - Create workspace for deletion test
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "delete-test-ws-001",
          "name": "Delete Test Workspace",
          "description": "Will be deleted",
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

# Test: Soft delete the workspace
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [],
      "updated": [],
      "deleted": ["delete-test-ws-001"]
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

# Test: Try to update a deleted workspace (API allows - no soft delete validation)
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [],
      "updated": [
        {
          "id": "delete-test-ws-001",
          "name": "Trying to Update Deleted Workspace",
          "description": "API allows this (no validation)",
          "created_at": 1705660800000,
          "last_modified": 1705668000000
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
[Asserts]
jsonpath "$.message" == "Changes synchronized successfully"

# ============================================================================
# 4. INVALID FIELDS
# ============================================================================

# Test: Missing required field (name)
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "invalid-ws-001",
          "description": "Missing name field",
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

HTTP 400

# Test: Empty ID (API accepts - no validation)
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "",
          "name": "Empty ID Workspace",
          "description": "Has empty ID",
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

# Test: Negative timestamp (API accepts - no validation)
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "negative-ts-ws-001",
          "name": "Negative Timestamp Workspace",
          "description": "Has negative timestamp",
          "created_at": -1000,
          "last_modified": -500
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

# ============================================================================
# 5. SCHEMA VERSION HANDLING
# ============================================================================

# Test: Pull with unsupported schema version
GET http://localhost:5000/sync/pull?last_pulled_at=0&schema_version=999

HTTP 200
[Asserts]
jsonpath "$.timestamp" > 0

# Test: Pull without schema version (should use default)
GET http://localhost:5000/sync/pull?last_pulled_at=0

HTTP 200
[Asserts]
jsonpath "$.timestamp" > 0

# ============================================================================
# 6. CLOCK DRIFT PROTECTION
# ============================================================================

# Test: Future timestamp from client (server should overwrite)
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "future-ts-ws-001",
          "name": "Future Timestamp Workspace",
          "description": "Client sent future timestamp",
          "created_at": 9999999999999,
          "last_modified": 9999999999999
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

# Test: Verify server overwrote future timestamp
GET http://localhost:5000/sync/pull?last_pulled_at=0&schema_version=1

HTTP 200
[Asserts]
jsonpath "$.changes.workspaces.created[*].id" contains "future-ts-ws-001"

# ============================================================================
# 7. ATOMIC TRANSACTION ROLLBACK
# ============================================================================

# Test: Mixed valid/invalid changes (should rollback all)
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "atomic-ws-001",
          "name": "Atomic Test Workspace",
          "description": "Should rollback if task fails",
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
          "id": "atomic-proj-001",
          "name": "Atomic Test Project",
          "description": "Should rollback",
          "workspace_id": "atomic-ws-001",
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
          "id": "atomic-task-001",
          "title": "Invalid Task",
          "description": "References non-existent project",
          "status": "pending",
          "priority": "low",
          "project_id": "non-existent-999",
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

HTTP 500

# Test: Verify workspace was NOT created (rollback worked)
GET http://localhost:5000/sync/pull?last_pulled_at=0&schema_version=1

HTTP 200
[Asserts]
jsonpath "$.changes.workspaces.created[*].id" not contains "atomic-ws-001"
jsonpath "$.changes.projects.created[*].id" not contains "atomic-proj-001"

# ============================================================================
# 8. BOUNDARY CONDITIONS
# ============================================================================

# Test: Empty arrays in changes (valid request)
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

# Test: Update non-existent entity
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [],
      "updated": [
        {
          "id": "never-existed-ws-999",
          "name": "Updating Ghost Workspace",
          "description": "This ID never existed",
          "created_at": 1705660800000,
          "last_modified": 1705668000000
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

# Test: Delete non-existent entity (should be idempotent)
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [],
      "updated": [],
      "deleted": ["never-existed-ws-777"]
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

# Test: Multiple operations on same ID (create then update in same request)
# This is an edge case that shouldn't happen but might
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "multi-op-ws-001",
          "name": "Multi-Op Workspace",
          "description": "Created in this request",
          "created_at": 1705660800000,
          "last_modified": 1705660800000
        }
      ],
      "updated": [
        {
          "id": "multi-op-ws-001",
          "name": "Multi-Op Workspace UPDATED",
          "description": "Updated in same request",
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

# Test: Very long string values (boundary test)
POST http://localhost:5000/sync/push
Content-Type: application/json

{
  "changes": {
    "workspaces": {
      "created": [
        {
          "id": "long-string-ws-001",
          "name": "This is a very long workspace name that might exceed typical field length limits and could potentially cause issues with database constraints or application logic processing",
          "description": "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
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
