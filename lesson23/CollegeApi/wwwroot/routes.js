const studentsUrl = "/students/all";
const groupsUrl = "/groups/all";

async function loadGroups() {
  const response = await fetch(groupsUrl);
  if (!response.ok) throw new Error("Не удалось загрузить группы");
  const groups = await response.json();

  document.querySelector("#groupId").innerHTML =
    '<option value="">Без группы</option>' +
    groups.map(group => `<option value="${group.id}">${escapeHtml(group.name)}</option>`).join("");

  document.querySelector("#groups").innerHTML = groups.map(group => `
    <li>
      ${group.id} — ${escapeHtml(group.name)}
      <button type="button" onclick="editGroup(${group.id}, '${escapeJs(group.name)}')">Изменить</button>
      <button type="button" class="delete" onclick="deleteGroup(${group.id})">Удалить</button>
    </li>
  `).join("");
}

async function loadStudents() {
  const response = await fetch(studentsUrl);
  if (!response.ok) throw new Error("Не удалось загрузить студентов");
  const students = await response.json();

  document.querySelector("#students").innerHTML = students.map(student => `
    <tr>
      <td>${student.id}</td>
      <td>${escapeHtml(student.firstName)}</td>
      <td>${escapeHtml(student.lastName)}</td>
      <td>${student.groupId ?? "—"}</td>
      <td>
        <button type="button" onclick="editStudent(${student.id}, '${escapeJs(student.firstName)}', '${escapeJs(student.lastName)}', ${student.groupId ?? "null"})">Изменить</button>
        <button type="button" class="delete" onclick="deleteStudent(${student.id})">Удалить</button>
      </td>
    </tr>
  `).join("");
}

document.querySelector("#student-form").addEventListener("submit", async event => {
  event.preventDefault();
  const payload = {
    firstName: document.querySelector("#firstName").value.trim(),
    lastName: document.querySelector("#lastName").value.trim(),
    groupId: document.querySelector("#groupId").value || null
  };

  try {
    const response = await fetch("/students/add", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });
    if (!response.ok) throw new Error(await responseMessage(response, "Не удалось добавить студента"));
    event.target.reset();
    await refreshLists();
  } catch (error) {
    showMessage(error.message);
  }
});

document.querySelector("#group-form").addEventListener("submit", async event => {
  event.preventDefault();
  const payload = { name: document.querySelector("#groupName").value.trim() };

  try {
    const response = await fetch("/groups/add", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });
    if (!response.ok) throw new Error(await responseMessage(response, "Не удалось добавить группу"));
    event.target.reset();
    await refreshLists();
  } catch (error) {
    showMessage(error.message);
  }
});

async function editStudent(id, firstName, lastName, groupId) {
  const newFirstName = prompt("Имя:", firstName);
  if (newFirstName === null) return;
  const newLastName = prompt("Фамилия:", lastName);
  if (newLastName === null) return;
  const newGroupId = prompt("ID группы (пусто — без группы):", groupId ?? "");
  if (newGroupId === null) return;

  const parsedGroupId = newGroupId.trim() === "" ? null : Number(newGroupId);
  if (parsedGroupId !== null && !Number.isInteger(parsedGroupId)) {
    showMessage("ID группы должен быть целым числом.");
    return;
  }

  const response = await fetch(`/students/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      firstName: newFirstName.trim(),
      lastName: newLastName.trim(),
      groupId: parsedGroupId
    })
  });

  if (!response.ok) {
    showMessage(await responseMessage(response, "Не удалось изменить студента"));
    return;
  }
  showMessage("");
  await refreshLists();
}

async function deleteStudent(id) {
  if (!confirm("Удалить студента?")) return;
  const response = await fetch(`/students/${id}`, { method: "DELETE" });
  if (!response.ok) {
    showMessage(await responseMessage(response, "Не удалось удалить студента"));
    return;
  }
  showMessage("");
  await refreshLists();
}

async function editGroup(id, name) {
  const newName = prompt("Название группы:", name);
  if (newName === null) return;
  const response = await fetch(`/groups/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ name: newName.trim() })
  });
  if (!response.ok) {
    showMessage(await responseMessage(response, "Не удалось изменить группу"));
    return;
  }
  showMessage("");
  await refreshLists();
}

async function deleteGroup(id) {
  if (!confirm("Удалить группу?")) return;
  const response = await fetch(`/groups/${id}`, { method: "DELETE" });
  if (!response.ok) {
    showMessage(await responseMessage(response, "Не удалось удалить группу"));
    return;
  }
  showMessage("");
  await refreshLists();
}

async function responseMessage(response, fallback) {
  const data = await response.json().catch(() => ({}));
  return data.message || fallback;
}

function showMessage(message) {
  document.querySelector("#message").textContent = message;
}

async function refreshLists() {
  await Promise.all([loadGroups(), loadStudents()]);
}

function escapeHtml(value) {
  return String(value ?? "").replace(/[&<>"']/g, character => ({
    "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
  }[character]));
}

function escapeJs(value) {
  return String(value ?? "").replace(/\\/g, "\\\\").replace(/'/g, "\\'")
    .replace(/\r/g, "\\r").replace(/\n/g, "\\n");
}

refreshLists().catch(error => showMessage(error.message));
