import requests
from mcp.server.fastmcp import FastMCP

mcp = FastMCP("project-management")

# Project的API工具

@mcp.tool()
async def create_project(id: int, name: str, description: str, ownerUid: int, createdAt: str) -> str:
    """创建新项目"""
    url = "http://localhost:7062/api/Project/Create"
    payload = {
        "id": id,
        "name": name,
        "description": description,
        "ownerUid": ownerUid,
        "createdAt": createdAt
    }
    resp = requests.post(url, json=payload)
    if resp.status_code == 200:
        return "项目创建成功"
    else:
        return f"创建失败: {resp.text}"

@mcp.tool()
async def get_all_projects() -> str:
    """获取所有项目列表"""
    url = "https://localhost:7062/api/Project/AllProjects"
    resp = requests.get(url)
    return resp.text if resp.status_code == 200 else f"获取失败: {resp.text}"

@mcp.tool()
async def get_project_by_id(id: int) -> str:
    """根据ID获取项目详情"""
    url = "https://localhost:7062/api/Project/{id}"
    resp = requests.get(url)
    return resp.text if resp.status_code == 200 else f"获取失败: {resp.text}"

@mcp.tool()
async def update_project(id: int, name: str, description: str, ownerUid: int, createdAt: str) -> str:
    """更新项目信息"""
    url = "https://localhost:7062/api/Project/Update/{id}"
    payload = {
        "id": id,
        "name": name,
        "description": description,
        "ownerUid": ownerUid,
        "createdAt": createdAt
    }
    resp = requests.put(url, json=payload)
    if resp.status_code == 200:
        return "项目信息更新成功"
    else:
        return f"更新失败: {resp.text}"

@mcp.tool()
async def delete_project(project_id: int) -> str:
    """删除项目"""
    url = "https://localhost:7062/api/Project/Delete/{id}"
    resp = requests.delete(url)
    return "删除成功" if resp.status_code == 200 else f"删除失败: {resp.text}"

@mcp.tool()
async def get_project_tasks(project_id: int) -> str:
    """获取项目下所有任务"""
    url = "https://localhost:7062/api/Project/{id}/Tasks"
    resp = requests.get(url)
    return resp.text if resp.status_code == 200 else f"获取失败: {resp.text}"

# Task的API工具

@mcp.tool()
async def create_task(id: int, title: str, description: str, deadline: str, status: int, priority: int, projectId: int, assignedUid: int, createUserId: int, dependencyTaskIds: list) -> str:
    """为指定项目创建任务"""
    url = "https://localhost:7062/api/Task/Add"
    payload = {
        "id": id,
        "title": title,
        "description": description,
        "deadline": deadline,
        "status": status,
        "priority": priority,
        "projectId": projectId,
        "assignedUid": assignedUid,
        "createUserId": createUserId,
        "dependencyTaskIds": dependencyTaskIds
    }
    resp = requests.post(url, json=payload)
    if resp.status_code == 200:
        return "任务创建成功"
    else:
        return f"创建失败: {resp.text}"

@mcp.tool()
async def update_task(id: int, title: str, description: str, deadline: str, status: int, priority: int, projectId: int, assignedUid: int, createUserId: int, dependencyTaskIds: list) -> str:
    """更新任务信息"""
    url = "https://localhost:7062/api/Task/Update"
    payload = {
        "id": id,
        "title": title,
        "description": description,
        "deadline": deadline,
        "status": status,
        "priority": priority,
        "projectId": projectId,
        "assignedUid": assignedUid,
        "createUserId": createUserId,
        "dependencyTaskIds": dependencyTaskIds
    }
    resp = requests.put(url, json=payload)
    if resp.status_code == 200:
        return "任务信息更新成功"
    else:
        return f"更新失败: {resp.text}"

@mcp.tool()
async def delete_task(id: int) -> str:
    """删除任务"""
    url = "https://localhost:7062/api/Task/Delete/{id}"
    resp = requests.delete(url)
    return "删除成功" if resp.status_code == 200 else f"删除失败: {resp.text}"

@mcp.tool()
async def get_comment(id: int) -> str:
    """根据评论id获取评论内容"""
    url = "https://localhost:7062/api/Comment/{id}"
    resp = requests.get(url)
    if resp.status_code == 200:
        return resp.text
    elif resp.status_code == 404:
        return "评论不存在"
    else:
        return f"获取失败: {resp.text}"

@mcp.tool()
async def add_comment(commentId: int, taskId: int, userId: int, content: str, createdTime: str) -> str:
    """为任务添加评论"""
    url = "https://localhost:7062/api/Comment/Add"
    payload = {
        "commentId": commentId,
        "taskId": taskId,
        "userId": userId,
        "content": content,
        "createdTime": createdTime
    }
    resp = requests.post(url, json=payload)
    if resp.status_code == 200:
        return "评论添加成功"
    else:
        return f"添加失败: {resp.text}"

@mcp.tool()
async def get_task_comments(id: int) -> str:
    """获取任务下所有评论"""
    url = "https://localhost:7062/api/Task/{id}/Comments"
    resp = requests.get(url)
    return resp.text if resp.status_code == 200 else f"获取失败: {resp.text}"