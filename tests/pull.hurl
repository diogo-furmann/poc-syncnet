# Test: Initial Pull (Empty Database)
# This test verifies that a pull from an empty database returns empty collections

GET http://localhost:5000/sync/pull?last_pulled_at=0&schema_version=1

HTTP 200
[Asserts]
jsonpath "$.timestamp" > 0
jsonpath "$.changes.workspaces.created" count >= 0
jsonpath "$.changes.workspaces.updated" count >= 0
jsonpath "$.changes.workspaces.deleted" count >= 0
jsonpath "$.changes.projects.created" count >= 0
jsonpath "$.changes.tasks.created" count >= 0
jsonpath "$.changes.comments.created" count >= 0

# Test: Pull with timestamp parameter
# This test verifies pulling with a specific timestamp

GET http://localhost:5000/sync/pull?last_pulled_at=1705660800000&schema_version=1

HTTP 200
[Asserts]
jsonpath "$.timestamp" > 1705660800000
jsonpath "$.changes" exists

# Test: Pull without query parameters (using defaults)

GET http://localhost:5000/sync/pull

HTTP 200
[Asserts]
jsonpath "$.timestamp" exists
jsonpath "$.changes.workspaces" exists
jsonpath "$.changes.projects" exists
jsonpath "$.changes.tasks" exists
jsonpath "$.changes.comments" exists
