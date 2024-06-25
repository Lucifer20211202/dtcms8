<template>
	<div class="mainbody">
		<dt-location :data="[{title:'会员管理'},{title:'会员组编辑'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item label="是否启用">
							<el-switch :active-value="0" :inactive-value="1" v-model="datas.model.status"></el-switch>
						</el-form-item>
						<el-form-item prop="title" label="会员组名">
							<el-input v-model="datas.model.title" placeholder="必填，128字符内"></el-input>
						</el-form-item>
						<el-form-item prop="grade" label="等级排序">
							<el-input v-model="datas.model.grade" placeholder="必填，等级排序(按顺序升级)">
								<template #append>级</template>
							</el-input>
						</el-form-item>
						<el-form-item prop="amount" label="预 存 款">
							<el-input v-model="datas.model.amount" placeholder="必填，升级所需的预存金额">
								<template #append>元</template>
							</el-input>
						</el-form-item>
						<el-form-item prop="exp" label="经 验 值">
							<el-input v-model="datas.model.exp" placeholder="必填，升级所需经验值"></el-input>
						</el-form-item>
						<el-form-item label="自动升级">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.isUpgrade"
								@change="val=>{val==1,datas.model.isPaid=0}"></el-switch>
						</el-form-item>
						<el-form-item label="注册默认">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.isDefault"></el-switch>
						</el-form-item>
						<el-form-item label="创建时间" v-if="datas.model.id>0">
							{{datas.model.addBy}} {{datas.model.addTime}}
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
	const policyTableRef = ref(null)
	
	//接收props传值
	const props = defineProps({
		id: 0
	})
	
	//定义页面属性
	const datas = reactive({
		loading: false,
		multipleSelection: [],
		model: {
			id: 0,
			title: null,
			grade: 1,
			amount: 0,
			exp: 0,
			isUpgrade: 1,
			isDefault: 0,
			status: 0,
			addBy: null,
			addTime: null,
		},
		rules: {
			title: [
				{ required: true, message: '请输入用户组名称', trigger: 'blur' }
			],
			grade: [
				{ required: true, message: '请输入等级排序数字', trigger: 'blur' },
				{ pattern: /^[+]{0,1}(\d+)$/, message: '等级排序只能为正整数', trigger: 'blur' }
			],
			amount: [
				{ required: true, message: '请输入预存金额(元)', trigger: 'blur' },
				{ pattern: /(^[1-9]([0-9]+)?(\.[0-9]{1,2})?$)|(^(0){1}$)|(^[0-9]\.[0-9]([0-9])?$)/, message: '请输入正确金额，可保留两位小数', trigger: 'blur' }
			],
			exp: [
				{ required: true, message: '请输入升级所需的经验值', trigger: 'blur' },
				{ pattern: /^[+]{0,1}(\d+)$/, message: '经验值只能为正整数', trigger: 'blur' }
			],
		},
	})
	
	//初始化数据
	const initData = async() => {
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/member/group/${props.id}`,
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
						url: `/admin/member/group/${datas.model.id}`,
						data: datas.model,
						successMsg: '修改会员组已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							proxy.$common.linkUrl('/member/group/list') //跳转加列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/member/group',
						data: datas.model,
						successMsg: '新增会员组已成功',
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