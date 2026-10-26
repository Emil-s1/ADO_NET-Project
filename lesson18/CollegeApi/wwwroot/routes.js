const api = "/api";

async function loadGroups() {
  const select = document.querySelector("#groupId");
  try {
    const response = await fetch(`${api}/groups`);
    if (!response.ok) throw new Error("Не удалось загрузить группы");
    const groups = await response.json();
    select.innerHTML = '<option value="">Без группы</option>' +
      groups.map(group => `<option value="${group.id}">${escapeHtml(group.name)}</option>`).join("");
  } catch (error) {
    document.querySelector("#message").textContent = error.message;
  }
}

async function loadStudents() {
  const response = await fetch(`${api}/students`);
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
    </tr>`).join("");
}

document.querySelector("#student-form").addEventListener("submit", async event => {
  event.preventDefault();
  const message = document.querySelector("#message");
  message.textContent = "";
  const payload = {
    firstName: document.querySelector("#firstName").value.trim(),
    lastName: document.querySelector("#lastName").value.trim(),
    groupId: document.querySelector("#groupId").value || null
  };
  try {
    const response = await fetch(`${api}/student`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });
    if (!response.ok) {
      const result = await response.json().catch(() => ({}));
      throw new Error(result.message || "Не удалось создать студента");
    }
    event.target.reset();
    await loadStudents();
  } catch (error) {
    message.textContent = error.message;
  }
});

async function editStudent(id, firstName, lastName, groupId) {
  const updatedFirstName = prompt("Введите имя:", firstName);
  if (updatedFirstName === null) return;
  const updatedLastName = prompt("Введите фамилию:", lastName);
  if (updatedLastName === null) return;
  const updatedGroup = prompt("Введите ID группы или оставьте пустым:", groupId ?? "");
  if (updatedGroup === null) return;

  const payload = {
    firstName: updatedFirstName.trim(),
    lastName: updatedLastName.trim(),
    groupId: updatedGroup.trim() === "" ? null : Number(updatedGroup)
  };

  if (!payload.firstName || !payload.lastName ||
      (payload.groupId !== null && !Number.isInteger(payload.groupId))) {
    document.querySelector("#message").textContent = "Проверьте введённые данные.";
    return;
  }

  const response = await fetch(`${api}/student/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload)
  });

  if (!response.ok) {
    document.querySelector("#message").textContent = "Не удалось изменить студента.";
    return;
  }

  document.querySelector("#message").textContent = "";
  await loadStudents();
}

async function deleteStudent(id) {
  if (!confirm("Удалить этого студента?")) return;
  const response = await fetch(`${api}/student/${id}`, { method: "DELETE" });
  if (!response.ok) {
    document.querySelector("#message").textContent = "Не удалось удалить студента.";
    return;
  }
  await loadStudents();
}

function escapeHtml(value) {
  return String(value ?? "").replace(/[&<>"']/g, character => ({
    "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
  }[character]));
}

function escapeJs(value) {
  return String(value ?? "").replace(/\\/g, "\\\\").replace(/'/g, "\\'").replace(/\r/g, "\\r").replace(/\n/g, "\\n");
}

loadGroups();
loadStudents().catch(error => {
  document.querySelector("#message").textContent = error.message;
});
