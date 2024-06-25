<template>
	<common-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
		<el-tab-pane label="修改密码" name="info">
			<el-form-item prop="password" label="当前密码">
				<el-input show-password v-model="datas.model.password" placeholder="必填，最少6位英文字母"></el-input>
			</el-form-item>
			<el-form-item prop="newPassword" label="新 密 码">
				<el-input show-password v-model="datas.model.newPassword" placeholder="必填，最少6位英文字母"></el-input>
			</el-form-item>
			<el-form-item prop="confirmPassword" label="确认密码">
				<el-input show-password v-model="datas.model.confirmPassword" placeholder="必填，最少6位英文字母"></el-input>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" :loading="datas.btnLoading" @click="submitForm">确认修改</el-button>
				<el-button @click="common.back(-1)">返回上一页</el-button>
			</el-form-item>
		</el-tab-pane>
	</common-form-box>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//页面SEO设置
	useSeoMeta({
		title: `修改密码 - ${siteConfig.seoKeyword}`,
		ogTitle: siteConfig.seoKeyword,
		description: siteConfig.seoDescription,
		ogDescription: siteConfig.seoDescription,
	})
	//Ref对象
	const editFormRef = ref(null)
	//自定义表单验证，必须放在datas之前
	const validatePass = (rule, value, callback) => {
		if (value == null || value == '') {
			callback(new Error('请输入确认密码'))
		} else if (value !== datas.model.newPassword) {
			callback(new Error("两次输入密码不一致"))
		} else {
			callback()
		}
	}
	//定义页面属性
	const datas = reactive({
		btnLoading: false,
		model: {
			password: null,
			newPassword: null,
			confirmPassword: null,
		},
		rules: {
			password: [
				{ required: true, message: '请输入当前登录密码', trigger: 'blur' },
				{ min: 6, max: 128, message: '密码长度在至少6位', trigger: 'blur' },
				{ pattern: /^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,20}$/, message: '必须英文字母数字组合', trigger: 'blur' }
			],
			newPassword: [
				{ required: true, message: '请输入新密码', trigger: 'blur' },
				{ min: 6, max: 128, message: '密码长度在至少6位', trigger: 'blur' },
				{ pattern: /^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,20}$/, message: '必须英文字母数字组合', trigger: 'blur' }
			],
			confirmPassword: [
				{ required: true, validator: validatePass, trigger: 'blur' }
			],
		}
	})
	
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				useHttp({
					method: 'put',
					url: `/account/member/password`,
					data: datas.model,
					successMsg: '密码修改成功',
					beforeSend() {
						datas.btnLoading = true
					},
					success(res) {
						editFormRef.value.form.resetFields()
					},
					complete() {
						datas.btnLoading = false
					}
				})
			}
		})
	}
</script>