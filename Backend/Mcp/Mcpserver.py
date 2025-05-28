import requests
import json
from mcp.server.fastmcp import FastMCP
from fastapi import Request, HTTPException
from jose import JWTError, jwt
from typing import Dict

# 从 appsettings.json 加载 JWT 配置
with open("appsettings.json", "r") as f:
    jwt_config = json.load(f)["Jwt"]

SECRET_KEY = jwt_config["Key"]
ALGORITHM = "HS256"
AUDIENCE = jwt_config["Audience"]
ISSUER = jwt_config["Issuer"]

# 用户ID -> client 绑定
user_clients: Dict[int, str] = {}

mcp = FastMCP("project-management")

async def get_user_id_from_token(request: Request) -> int:
    token = request.headers.get("Authorization")
    if not token or not token.startswith("Bearer "):
        raise HTTPException(status_code=401, detail="缺少或错误的认证信息")

    token = token.split(" ")[1]
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM], audience=AUDIENCE, issuer=ISSUER)
        user_id = payload.get("uid")
        if user_id is None:
            raise HTTPException(status_code=401, detail="Token中不包含用户ID")
        if user_id not in user_clients:
            user_clients[user_id] = f"Client for user {user_id}"
        return user_id
    except JWTError as e:
        raise HTTPException(status_code=401, detail=f"Token解码失败: {str(e)}")

# 所有工具函数使用 request + get_user_id_from_token
@mcp.tool()
async def create_project(request: Request, id: int, name: str, description: str, ownerUid: int, createdAt: str) -> str:
    user_id = await get_user_id_from_token(request)
    url = "http://localhost:7062/api/Project/Create"
    payload = {
        "id": id,
        "name": name,
        "description": description,
        "ownerUid": ownerUid,
        "createdAt": createdAt
    }
    resp = requests.post(url, json=payload, headers={"Authorization": request.headers.get("Authorization")})
    return "项目创建成功" if resp.status_code == 200 else f"创建失败: {resp.text}"

@mcp.tool()
async def get_all_projects(request: Request) -> str:
    user_id = await get_user_id_from_token(request)
    url = "https://localhost:7062/api/Project/AllProjects"
    resp = requests.get(url, headers={"Authorization": request.headers.get("Authorization")})
    return resp.text if resp.status_code == 200 else f"获取失败: {resp.text}"

@mcp.tool()
async def get_project_by_id(request: Request, id: int) -> str:
    user_id = await get_user_id_from_token(request)
    url = f"https://localhost:7062/api/Project/{id}"
    resp = requests.get(url, headers={"Authorization": request.headers.get("Authorization")})
    return resp.text if resp.status_code == 200 else f"获取失败: {resp.text}"

@mcp.tool()
async def update_project(request: Request, id: int, name: str, description: str, ownerUid: int, createdAt: str) -> str:
    user_id = await get_user_id_from_token(request)
    url = f"https://localhost:7062/api/Project/Update/{id}"
    payload = {
        "id": id,
        "name": name,
        "description": description,
        "ownerUid": ownerUid,
        "createdAt": createdAt
    }
    resp = requests.put(url, json=payload, headers={"Authorization": request.headers.get("Authorization")})
    return "项目信息更新成功" if resp.status_code == 200 else f"更新失败: {resp.text}"

@mcp.tool()
async def delete_project(request: Request, id: int) -> str:
    user_id = await get_user_id_from_token(request)
    url = f"https://localhost:7062/api/Project/Delete/{id}"
    resp = requests.delete(url, headers={"Authorization": request.headers.get("Authorization")})
    return "删除成功" if resp.status_code == 200 else f"删除失败: {resp.text}"

@mcp.tool()
async def get_project_tasks(request: Request, id: int) -> str:
    user_id = await get_user_id_from_token(request)
    url = f"https://localhost:7062/api/Project/{id}/Tasks"
    resp = requests.get(url, headers={"Authorization": request.headers.get("Authorization")})
    return resp.text if resp.status_code == 200 else f"获取失败: {resp.text}"

@mcp.tool()
async def create_task(request: Request, id: int, title: str, description: str, deadline: str, status: int, priority: int, projectId: int, assignedUid: int, createUserId: int, dependencyTaskIds: list) -> str:
    user_id = await get_user_id_from_token(request)
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
    resp = requests.post(url, json=payload, headers={"Authorization": request.headers.get("Authorization")})
    return "任务创建成功" if resp.status_code == 200 else f"创建失败: {resp.text}"

@mcp.tool()
async def update_task(request: Request, id: int, title: str, description: str, deadline: str, status: int, priority: int, projectId: int, assignedUid: int, createUserId: int, dependencyTaskIds: list) -> str:
    user_id = await get_user_id_from_token(request)
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
    resp = requests.put(url, json=payload, headers={"Authorization": request.headers.get("Authorization")})
    return "任务信息更新成功" if resp.status_code == 200 else f"更新失败: {resp.text}"

@mcp.tool()
async def delete_task(request: Request, id: int) -> str:
    user_id = await get_user_id_from_token(request)
    url = f"https://localhost:7062/api/Task/Delete/{id}"
    resp = requests.delete(url, headers={"Authorization": request.headers.get("Authorization")})
    return "删除成功" if resp.status_code == 200 else f"删除失败: {resp.text}"

@mcp.tool()
async def get_comment(request: Request, id: int) -> str:
    user_id = await get_user_id_from_token(request)
    url = f"https://localhost:7062/api/Comment/{id}"
    resp = requests.get(url, headers={"Authorization": request.headers.get("Authorization")})
    if resp.status_code == 200:
        return resp.text
    elif resp.status_code == 404:
        return "评论不存在"
    else:
        return f"获取失败: {resp.text}"

@mcp.tool()
async def add_comment(request: Request, commentId: int, taskId: int, userId: int, content: str, createdTime: str) -> str:
    user_id = await get_user_id_from_token(request)
    url = "https://localhost:7062/api/Comment/Add"
    payload = {
        "commentId": commentId,
        "taskId": taskId,
        "userId": userId,
        "content": content,
        "createdTime": createdTime
    }
    resp = requests.post(url, json=payload, headers={"Authorization": request.headers.get("Authorization")})
    return "评论添加成功" if resp.status_code == 200 else f"添加失败: {resp.text}"

@mcp.tool()
async def get_task_comments(request: Request, id: int) -> str:
    user_id = await get_user_id_from_token(request)
    url = f"https://localhost:7062/api/Task/{id}/Comments"
    resp = requests.get(url, headers={"Authorization": request.headers.get("Authorization")})
    return resp.text if resp.status_code == 200 else f"获取失败: {resp.text}"