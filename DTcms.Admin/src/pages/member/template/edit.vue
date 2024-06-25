<template>
	<div class="mainbody">
		<dt-location :data="[{title:'会员设置'},{title:'编辑模板'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item prop="type" label="模板类型">
							<el-radio-group v-model="datas.model.type">
								<el-radio-button label="1">邮件</el-radio-button>
								<el-radio-button label="2">短信</el-radio-button>
								<el-radio-button label="3">微信</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item prop="callIndex" label="调用名称">
							<el-input v-model="datas.model.callIndex" placeholder="必填，只允许英文数字下划线"></el-input>
						</el-form-item>
						<el-form-item prop="title" label="模板标题">
							<el-input v-model="datas.model.title" placeholder="必填，128字符内"></el-input>
						</el-form-item>
						<el-form-item prop="templateId" label="模板标识">
							<el-input v-model="datas.model.templateId" placeholder="邮箱选填，短信或微信平台必填"></el-input>
						</el-form-item>
						<el-form-item prop="content" label="模板内容">
							<el-input type="textarea" :rows="5" v-model="datas.model.content" placeholder="必填，支持HTML"></el-input>
						</el-form-item>
						<el-form-item label="填写说明">
							<div class="remark">
								阿里云的模板内容请填写JSON格式字符串，格式如：{"code":"{code}"}<br />
								腾讯云的模板内容请用英文逗号分隔开，且变量顺序要和报备的一致；<br />
							</div>
						</el-form-item>
						<el-form-item label="更新时间" v-if="datas.model.updateTime">
							{{datas.model.updateTime}}
						</el-form-item>
					</div>
				</el-tab-pane>
			</dt-form-box>
		</div>
		
		<div class="footer-box">
			<div class="footer-btn">
				<el-button type="primary" :loading="datas.loading" @click="submitForm">确认保存</el-button>
				<el-button plain @click="$common.back()">返回上一页</el-button>
			</div>
		</div>
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtFormBox from '../../../components/layout/DtFormBox.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//Ref对象
	const editFormRef = ref(null)
	
	//接收props传值
	const props = defineProps({
		id: 0
	})
	
	//定义页面属性
	const datas = reactive({
		loading: false,
		model: {
			id: 0,
			type: null,
			callIndex: null,
			title: null,
			templateId: null,
			content: null,
			updateTime: null,
		},
		rules: {
			type: [
				{ required: true, message: '请选择模板类型', trigger: 'change' }
			],
			callIndex: [
				{ required: true, message: '请输入模板调用名称', trigger: 'blur' },
				{ pattern: /^[a-zA-Z0-9_]*$/, message: '只能是字母数字下划线', trigger: 'blur' }
			],
			title: [
				{ required: true, message: '请输入模板标题', trigger: 'blur' }
			],
			content: [
				{ required: true, message: '请输入模板内容', trigger: 'blur' }
			]
		},
	})
	
	//初始化数据
	const initData = async() => {
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/notify/template/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		}
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				//Id大于0则修改，否则添加
				if (datas.model.id > 0) {
					proxy.$api.request({
						method: 'put',
						url: `/admin/notify/template/${datas.model.id}`,
						data: datas.model,
						successMsg: '消息模板修改成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							proxy.$common.linkUrl('/member/template/list') //跳转加列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/notify/template',
						data: datas.model,
						successMsg: '消息模板添加成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							editFormRef.value.form.resetFields() //重置表单
						},
						complete() {
							datas.loading = false
						}
					})
				}
			}
		})
	}
	
	//运行初始化
	initData()
</script>